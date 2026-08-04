using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Web.ViewModels.Account;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Application.Abstractions;

namespace Portfolio.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAuditLogger _auditLogger;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        IAuditLogger auditLogger)
    {
        _signInManager = signInManager;
        _auditLogger = auditLogger;
    }

    [HttpGet("/account/login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("/account/login")]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("admin-login")]
    public async Task<IActionResult> Login(
        LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _signInManager
            .PasswordSignInAsync(
                model.Email,
                model.Password,
                isPersistent: false,
                lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            await _auditLogger.WriteAsync(
                action: "AdminLoginLockedOut",
                succeeded: false,
                details: "Inicio de sesión rechazado porque la cuenta de administrador está bloqueada.");

            ModelState.AddModelError(
                string.Empty,
                "La cuenta ha sido bloqueada debido a múltiples intentos fallidos. Por favor, inténtelo de nuevo nunca jamas, gracias.");

            return View(model);
        }

        if (!result.Succeeded)
        {
            await _auditLogger.WriteAsync(
                action: "AdminLoginFailed",
                succeeded: false,
                details: "Intento de inicio de sesión de administrador inválido.");

            ModelState.AddModelError(
                string.Empty,
                "Credenciales incorrectas.");

            return View(model);
        }

        await _auditLogger.WriteAsync(
            action: "AdminLoginSucceeded",
            succeeded: true,
            details: "Inicio de sesión de administrador exitoso.");

        return RedirectToAction(
            "Index",
            "Home",
            new { area = "Admin" });
    }

    [HttpPost("/account/logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction(
            "Index",
            "Home");
    }
}