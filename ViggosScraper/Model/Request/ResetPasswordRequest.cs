using System.ComponentModel.DataAnnotations;

namespace ViggosScraper.Model.Request;

public class ResetPasswordRequest
{
    [Required] public string PhoneNumber { get; set; } = null!;
}