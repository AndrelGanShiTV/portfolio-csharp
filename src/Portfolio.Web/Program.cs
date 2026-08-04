using Portfolio.Application.Services;
using Portfolio.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Portfolio.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Domain.Entities;
using Portfolio.Application.Abstractions;
using Portfolio.Infrastructure.Auditing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure the database context
var databaseProvider =
    builder.Configuration["DatabaseProvider"]
    ?? throw new InvalidOperationException(
        "DatabaseProvider is not configured.");

var connectionString =
    builder.Configuration.GetConnectionString(
        "DefaultConnection")
    ?? throw new InvalidOperationException(
        "DefaultConnection is not configured.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    switch (databaseProvider.ToLowerInvariant())
    {
        case "sqlite":
            options.UseSqlite(connectionString,
                sqliteOptions =>
                    sqliteOptions.MigrationsAssembly(
                        "Portfolio.Migrations.Sqlite"));
            break;

        case "postgresql":
            options.UseNpgsql(connectionString,
                postgreSqlOptions =>
                    postgreSqlOptions.MigrationsAssembly(
                        "Portfolio.Migrations.PostgreSql"));
            break;

        default:
            throw new InvalidOperationException(
                $"Unsupported database provider: {databaseProvider}");
    }
});

// Configure Identity
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Configure application cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "Portfolio.Admin.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;

    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;

    options.LoginPath = "/account/Login";
    options.AccessDeniedPath = "/account/AccessDenied";
});

// Add HttpContextAccessor for accessing HTTP context in services
builder.Services.AddHttpContextAccessor();

// Register application services
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IExperienceService, ExperienceService>();
builder.Services.AddScoped<IContactMessageService, ContactMessageService>();
builder.Services.AddScoped<IAuditLogger, AuditLogger>();

// Configure rate limiting for the contact form
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode =
        StatusCodes.Status429TooManyRequests;

    options.AddPolicy(
        policyName: "contact-form",
        partitioner: httpContext =>
        {
            var ipAddress =
                httpContext.Connection.RemoteIpAddress?.ToString()
                ?? "unknown";

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: ipAddress,
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 2,
                    Window = TimeSpan.FromMinutes(30),
                    QueueLimit = 0,
                    AutoReplenishment = true
                });
        });

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType =
            "text/plain; charset=utf-8";

        await context.HttpContext.Response.WriteAsync(
            "Has enviado demasiados mensajes. Intenta nuevamente en unos minutos.",
            cancellationToken);
    };
});

// Configure rate limiting for the admin login
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("admin-login", httpContext =>
    {
        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: clientIp,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            });
    });

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "text/plain";

        await context.HttpContext.Response.WriteAsync(
            "Demasiados intentos de inicio de sesión. Por favor, inténtelo de nuevo más tarde.",
            cancellationToken);
    };
});

// Configure health checks for the application
builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

// Configure forwarded headers for reverse proxy scenarios
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;

    options.ForwardLimit = 1;

    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

// Configure Identity options for account lockout
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
});

var app = builder.Build();

// Seed the database with initial data
await using var scope = app.Services.CreateAsyncScope();

var services = scope.ServiceProvider;

var logger =
    services.GetRequiredService<ILogger<Program>>();

try
{
    var context =
        services.GetRequiredService<AppDbContext>();

    var userManager =
        services.GetRequiredService<UserManager<ApplicationUser>>();

    var roleManager =
        services.GetRequiredService<RoleManager<IdentityRole>>();

    await context.Database.MigrateAsync();

    await DbSeeder.SeedAsync(
        context,
        userManager,
        roleManager,
        builder.Configuration);
}
catch (Exception exception)
{
    logger.LogCritical(
        exception,
        "Database migration or seeding failed.");

    throw;
}

// Enable forwarded headers middleware
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/500");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}
app.UseStatusCodePagesWithReExecute("/Error/{0}");

// Enable HTTPS redirection, static files, and routing
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Enable rate limiting middleware
app.UseRateLimiter();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Map static assets for the application
app.MapStaticAssets();

// Map controller routes for areas and default route
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Map health check endpoint
app.MapHealthChecks("/health");

// Run the application
app.Run();
