using System.Net;
using ViggosScraper.Middleware;

namespace ViggosScraper.Model.Exception;

public class NotFoundException : HttpException
{
    public NotFoundException(string message) : base(HttpStatusCode.NotFound, message)
    {
    }
}