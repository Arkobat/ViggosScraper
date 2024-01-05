using System.Net;

namespace ViggosScraper.Model.Exception;

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