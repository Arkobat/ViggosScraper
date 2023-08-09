using System.Net;
using System.Text.Json;

namespace ViggosScraper.Middleware;

public class ExceptionMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (HttpException e)
        {
            _logger.LogDebug("Error: {Message}", e.Message);
            await HandleExceptionAsync(context, e.HttpStatus, e.Message);
        }
        catch (NotImplementedException e)
        {
            _logger.LogError(e, "Unexpected error");
            await HandleExceptionAsync(context, HttpStatusCode.NotImplemented, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error");
            await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, e.Message);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, HttpStatusCode status, string message)
    {
        await HandleExceptionAsync(context, (int) status, message);
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
    public int StatusCode { get; set; }
    public string Message { get; set; } = null!;

    public override string ToString()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}

public class HttpException : System.Exception
{
    public int HttpStatus { get; }
    public override string Message { get; }

    public HttpException(HttpStatusCode httpStatus, string message)
    {
        HttpStatus = (int) httpStatus;
        Message = message;
    }
}