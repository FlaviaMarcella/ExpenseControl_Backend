using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseControl.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            // Log the exception
            logger.LogError(ex, "An unhandled exception occurred while processing the request.");

            var (statusCode, title, detail) = ex switch
            {
                InvalidOperationException => (HttpStatusCode.BadRequest, "Invalid operation", ex.Message),
                ArgumentNullException => (HttpStatusCode.BadRequest, "Missing required data", ex.Message),
                _ => (HttpStatusCode.InternalServerError, "An error occurred while processing your request.",
                    "Please try again later or contact support if the issue persists.")
            };

            var problemDetails = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = title,
                Detail = detail
            };

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}