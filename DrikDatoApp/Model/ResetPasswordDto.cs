using System.Text.Json.Serialization;

namespace DrikDatoApp.Model;

public class ResetPasswordDto
{
    [JsonPropertyName("status")]
    public required int Status { get; set; }
    
    [JsonPropertyName("msg")]
    public required string Message { get; set; }
}
