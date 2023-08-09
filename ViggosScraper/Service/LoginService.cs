using System.Text.Json;
using ViggosScraper.Model;

namespace ViggosScraper.Service;

public class LoginService
{
    private readonly HttpClient _httpClient;
    private readonly SymbolService _symbolService;

    public LoginService(HttpClient httpClient, SymbolService symbolService)
    {
        _httpClient = httpClient;
        _symbolService = symbolService;
    }

    public async Task<LoginResponse> Login(string phoneNumber, string password)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var url = $"https://www.drikdato.app/_service/service.php?ts={now}";

        var formContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("action", "login"),
            new KeyValuePair<string, string>("mobile", phoneNumber),
            new KeyValuePair<string, string>("password", password),
        });
        var response = await _httpClient.PostAsync(url, formContent);
        var json = await response.Content.ReadAsStringAsync();

        var res = JsonSerializer.Deserialize<ViggoLoginResponse>(json, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        })!;

        return ConvertResponse(res);
    }

    public async Task<LoginResponse> Authenticate(string token)
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

        return ConvertResponse(res);
    }

    private LoginResponse ConvertResponse(ViggoLoginResponse response)
    {
        if (response.Status == 0)
        {
            return new LoginResponse
            {
                Success = false,
                Message = response.Msg
            };
        }

        var user = response.Player;
        var dates = user!.Dates
            .Select(d => DateOnly.ParseExact(d.DateFormatted, "dd-MM-yyyy HH:mm", null))
            .ToList();
        var symbols = _symbolService.GetSymbols(dates)
            .ToDictionary(d => d.Date, d => d);


        var totalDates = user.Dates.Count;
        var datoer = new List<Dato>();
        for (var i = totalDates - 1; i >= 0; i--)
        {
            var date = DateOnly.ParseExact(user.Dates[i].DateFormatted, "dd-MM-yyyy HH:mm", null);
            datoer.Add(new Dato()
            {
                Number = totalDates - i,
                Date = date,
                Symbol = symbols.GetValueOrDefault(date)?.Simple()
            });
        }

        return new LoginResponse
        {
            Success = true,
            Message = response.Msg,
            Token = user.Token,
            Profile = new User()
            {
                ProfileId = user.Id,
                Name = user.Alias,
                Krus = user.GlassNumber,
                AvatarUrl = user.Photo,
                Dates = datoer
            }
        };
    }

    public async Task ResetPassword(string phoneNumber)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var url = $"https://www.drikdato.app/_service/service.php?ts={now}";

        var formContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("action", "forgotpassword"),
            new KeyValuePair<string, string>("mobile", phoneNumber),
        });
        var response = await _httpClient.PostAsync(url, formContent);
        var json = await response.Content.ReadAsStringAsync();

        var res = JsonSerializer.Deserialize<ViggoLoginResponse>(json, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        })!;
    }
}