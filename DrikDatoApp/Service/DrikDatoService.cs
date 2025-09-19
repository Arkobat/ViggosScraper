using System.Text.Json;
using System.Text.Json.Serialization;
using DrikDatoApp.Model;

namespace DrikDatoApp.Service;

public interface IDrikDatoService
{
    Task<HighcoreDto> GetHighscore(DateOnly from = default, DateOnly to = default);
    Task<SearchDto> Search(string searchTerm);
    Task<GetUserResponse> GetUser(string userId);
    Task<GetSelfUserResponse> Login(string mobile, string password);
    Task<GetSelfUserResponse> Authorize(string token);
    Task<ResetPasswordDto> ResetPassword(string phoneNumber);
}

internal class DrikDatoService(IHttpClientFactory httpClientFactory) : IDrikDatoService
{
    private const string BaseUrl = "https://www.drikdato.app/_service/service.php";

    public async Task<HighcoreDto> GetHighscore(DateOnly from = default, DateOnly to = default)
    {
        var period = from == default || to == default 
            ? "alltime" 
            : $"{from:yyyy-MM-dd}|{to:yyyy-MM-dd}";

        var formData = new FormUrlEncodedContent([
            new("action", "highscores"),
            new("period", period)
        ]);

        return await PostAndDeserialize<HighcoreDto>(formData);
    }

    public async Task<SearchDto> Search(string searchTerm)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(searchTerm);

        var formData = new FormUrlEncodedContent([
            new("action", "search"),
            new("search", searchTerm)
        ]);

        return await PostAndDeserialize<SearchDto>(formData);
    }

    public async Task<GetUserResponse> GetUser(string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var formData = new FormUrlEncodedContent([
            new("action", "getplayer"),
            new("id", userId)
        ]);

        return await PostAndDeserialize<GetUserResponse>(formData);
    }

    public async Task<GetSelfUserResponse> Login(string mobile, string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mobile);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var formData = new FormUrlEncodedContent([
            new("action", "login"),
            new("mobile", mobile),
            new("password", password)
        ]);

        return await PostAndDeserialize<GetSelfUserResponse>(formData);
    }

    public async Task<GetSelfUserResponse> Authorize(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var formData = new FormUrlEncodedContent([
            new("action", "authorize"),
            new("token", token)
        ]);

        return await PostAndDeserialize<GetSelfUserResponse>(formData);
    }

    public async Task<ResetPasswordDto> ResetPassword(string phoneNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(phoneNumber);

        var formData = new FormUrlEncodedContent([
            new("action", "forgotpassword"),
            new("mobile", phoneNumber)
        ]);

        return await PostAndDeserialize<ResetPasswordDto>(formData);
    }

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };
    
    private async Task<T> PostAndDeserialize<T>(FormUrlEncodedContent formData) where T : class
    {
        using var httpClient = httpClientFactory.CreateClient();
        
        var response = await httpClient.PostAsync(BaseUrl, formData);
        response.EnsureSuccessStatusCode();

        var jsonContent = await response.Content.ReadAsStringAsync();
        
        var result = JsonSerializer.Deserialize<T>(jsonContent, JsonSerializerOptions);
        return result ?? throw new InvalidOperationException($"Failed to deserialize {typeof(T).Name} response");
    }
}
