using System.Net;
using ViggosScraper.Middleware;

namespace ViggosScraper.Model.Exception;

public class BadRequestException : HttpException
{
    public BadRequestException(string message) : base(HttpStatusCode.BadRequest, message)
    {
    }
}