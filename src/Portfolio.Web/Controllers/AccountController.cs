using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Web.ViewModels.Account;

namespace Portfolio.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(
        SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpGet("/account/login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("/account/login")]
    [ValidateAntiForgeryToken]
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
            ModelState.AddModelError(
                string.Empty,
                "La cuenta ha sido bloqueada debido a múltiples intentos fallidos. Por favor, inténtelo de nuevo nunca jamas, gracias.");

            return View(model);
        }

        if (!result.Succeeded)
        {
            ModelState.AddModelError(
                string.Empty,
                "Credenciales incorrectas.");

            return View(model);
        }

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