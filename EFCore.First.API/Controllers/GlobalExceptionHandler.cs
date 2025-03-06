using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
namespace EFCore.First.API.Controllers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly Serilog.ILogger _logger;
    public GlobalExceptionHandler(Serilog.ILogger logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        //// Set content type for response
        //httpContext.Response.ContentType = "application/json";

        // Create problem details
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path,
            Detail = "An unexpected error occurred."
        };

        _logger.Error(exception, "Unhandled exception occurred");
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        problemDetails.Status = StatusCodes.Status500InternalServerError;
        problemDetails.Title = "Internal Server Error";
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // Return true to indicate the exception was handled
        return true;
    }
}