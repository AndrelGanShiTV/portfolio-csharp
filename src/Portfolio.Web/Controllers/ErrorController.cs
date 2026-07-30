using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Portfolio.Web.Controllers;

public class ErrorController : Controller
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(
    ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    [Route("Error/404")]
    public IActionResult NotFoundPage()
    {
        Response.StatusCode = StatusCodes.Status404NotFound;

        return View("NotFound");
    }

    [Route("Error/500")]
    public IActionResult ServerError()
    {
        var exceptionFeature =
            HttpContext.Features
                .Get<IExceptionHandlerPathFeature>();

        if (exceptionFeature is not null)
        {
            _logger.LogError(
                exceptionFeature.Error,
                "Unhandled exception while processing {Path}",
                exceptionFeature.Path);
        }

        Response.StatusCode =
            StatusCodes.Status500InternalServerError;

        return View("ServerError");
    }
}