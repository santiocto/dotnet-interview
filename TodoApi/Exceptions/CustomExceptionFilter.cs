using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TodoApi.Exceptions;

public class CustomExceptionFilter : IExceptionFilter
{
    private readonly ILogger<CustomExceptionFilter> _logger;

    public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(
            context.Exception,
            "Unhandled exception caught by CustomExceptionFilter. Path: {Path}",
            context.HttpContext.Request.Path);

        var problem = new ProblemDetails
        {
            Title = "Se produjo un error inesperado.",
            Detail = "Ocurrió un error en el servidor al procesar la solicitud.",
            Status = (int)HttpStatusCode.InternalServerError,
            Instance = context.HttpContext.TraceIdentifier
        };

        context.Result = new ObjectResult(problem)
        {
            StatusCode = (int)HttpStatusCode.InternalServerError
        };

        context.ExceptionHandled = true;
    }
}
