namespace ViggosScraper.Model;

public class LoginResponse
{
    public required bool Success { get; set; }
    public required string Message { get; set; } = null!;
    public string? Token { get; set; }
    public User? Profile { get; set; }
}