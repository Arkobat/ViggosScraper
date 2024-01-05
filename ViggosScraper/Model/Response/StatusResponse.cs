namespace ViggosScraper.Model.Response;

public class StatusResponse
{
    public required bool Success { get; set; }
    public required string Message { get; set; } = null!;
}