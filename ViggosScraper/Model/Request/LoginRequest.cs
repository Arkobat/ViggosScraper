using System.ComponentModel.DataAnnotations;

namespace ViggosScraper.Model;

public class LoginRequest
{
    [Required] public string Username { get; set; } = null!;
    [Required] public string Password { get; set; } = null!;
}

public class AuthRequest
{
    [Required] public string Token { get; set; } = null!;
}