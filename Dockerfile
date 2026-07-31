FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Portfolio.slnx ./

COPY src/Portfolio.Domain/Portfolio.Domain.csproj src/Portfolio.Domain/
COPY src/Portfolio.Application/Portfolio.Application.csproj src/Portfolio.Application/
COPY src/Portfolio.Infrastructure/Portfolio.Infrastructure.csproj src/Portfolio.Infrastructure/
COPY src/Portfolio.Migrations.Sqlite/Portfolio.Migrations.Sqlite.csproj src/Portfolio.Migrations.Sqlite/
COPY src/Portfolio.Migrations.PostgreSql/Portfolio.Migrations.PostgreSql.csproj src/Portfolio.Migrations.PostgreSql/
COPY src/Portfolio.Web/Portfolio.Web.csproj src/Portfolio.Web/

RUN dotnet restore src/Portfolio.Web/Portfolio.Web.csproj

COPY . .

RUN dotnet publish src/Portfolio.Web/Portfolio.Web.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Portfolio.Web.dll"]