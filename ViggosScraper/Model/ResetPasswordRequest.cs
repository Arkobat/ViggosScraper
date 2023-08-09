using System.ComponentModel.DataAnnotations;

namespace ViggosScraper.Model;

public class ResetPasswordRequest
{
    [Required] public string PhoneNumber { get; set; } = null!;
}