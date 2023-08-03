using System.Text.Json;
using ViggosScraper.Model;

namespace ViggosScraper.Service;

public class LoginService
{
    private readonly HttpClient _httpClient;

    public LoginService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ViggoLoginResponse> Login(string username, string password)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var url = $"https://www.drikdato.app/_service/service.php?ts={now}";

        var formContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("action", "login"),
            new KeyValuePair<string, string>("mobile", username),
            new KeyValuePair<string, string>("password", password),
        });
        var response = await _httpClient.PostAsync(url, formContent);

        var json = await response.Content.ReadAsStringAsync();
        

        var res = JsonSerializer.Deserialize<ViggoLoginResponse>(json, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        })!;
        
        return res;
    }
    
    public async Task<ViggoLoginResponse> Authenticate(string token)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var url = $"https://www.drikdato.app/_service/service.php?ts={now}";

        var formContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("action", "authorize"),
            new KeyValuePair<string, string>("token", token),
        });
        var response = await _httpClient.PostAsync(url, formContent);

        var json = await response.Content.ReadAsStringAsync();
        
        var res = JsonSerializer.Deserialize<ViggoLoginResponse>(json, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        })!;
        
        return res;
    }
}
