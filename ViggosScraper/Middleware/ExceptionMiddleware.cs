using System.Net;
using System.Text.Json;
using ViggosScraper.Model.Exception;

namespace ViggosScraper.Middleware;

public class ExceptionMiddleware(ILogger<ExceptionMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (HttpException e)
        {
            await HandleExceptionAsync(context, e.HttpStatus, e.Message);
        }
        catch (NotImplementedException e)
        {
            logger.LogError(e, "Unexpected error");
            await HandleExceptionAsync(context, HttpStatusCode.NotImplemented, e.Message);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unexpected error");
            await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, e.Message);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, HttpStatusCode status, string message)
    {
        await HandleExceptionAsync(context, (int)status, message);
    }

    private async Task HandleExceptionAsync(HttpContext context, int status, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = status;
        await context.Response.WriteAsync(new ErrorDetails()
        {
            StatusCode = status,
            Message = message
        }.ToString());
    }
}

public class ErrorDetails
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    public int StatusCode { get; set; }
    public string Message { get; set; } = null!;


    public override string ToString()
    {
        return JsonSerializer.Serialize(this, JsonSerializerOptions);
    }
}