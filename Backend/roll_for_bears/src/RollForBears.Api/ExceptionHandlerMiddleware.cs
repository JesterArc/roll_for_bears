using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using RollForBears.Modules.Shared_Kernel.Exceptions;

namespace RollForBears.Api;

internal sealed class ExceptionHandlerMiddleware(
    RequestDelegate next, 
    ILogger<ExceptionHandlerMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");

            httpContext.Response.StatusCode = ex switch
            {
                DomainException exception => exception.ErrorCode,
                DbUpdateException => 400,
                _ => 500
            };
            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Title = "An error has occurred",
                    Detail = ex.Message,
                });
        }
    }
}