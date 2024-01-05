using System.Text.Json.Serialization;

namespace ViggosScraper.Model.Response;

public class LoginResponse : StatusResponse
{
    public string? Token { get; set; }
    public UserDto? Profile { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Permissions { get; set; }
}