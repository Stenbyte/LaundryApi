using System.Net;
using System.Text.Json;
using LaundryApi.Exceptions;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (CustomException ex)
        {
            await HandleExceptionAsync(context, ex.StatusCode, ex.Message, ex.ErrorDetails);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, (int)HttpStatusCode.InternalServerError, "Something went wrong", ex.Message);
        }

    }
    private static Task HandleExceptionAsync(HttpContext context, int statusCode, string message, object? details)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new { message, details };
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}