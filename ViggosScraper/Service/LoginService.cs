﻿using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ViggosScraper.Database;
using ViggosScraper.Middleware;
using ViggosScraper.Model;
using ViggosScraper.Model.Request;
using ViggosScraper.Model.Response;

namespace ViggosScraper.Service;

public class LoginService
{
    private readonly HttpClient _httpClient;
    private readonly SymbolService _symbolService;
    private readonly UserService _userService;
    private readonly ViggosDb _dbContext;
    private readonly HttpSession _httpSession;
    private readonly ICache<string, AuthResponse> _authCache;

    public LoginService(HttpClient httpClient, SymbolService symbolService, UserService userService, ViggosDb dbContext,
        HttpSession httpSession, ICache<string, AuthResponse> authCache)
    {
        _httpClient = httpClient;
        _symbolService = symbolService;
        _userService = userService;
        _dbContext = dbContext;
        _httpSession = httpSession;
        _authCache = authCache;
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

        return await ConvertResponse(res);
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

        return await ConvertResponse(res);
    }

    private async Task<LoginResponse> ConvertResponse(ViggoLoginResponse response)
    {
        // Login failed
        if (response.Status == 0)
        {
            return new LoginResponse
            {
                Success = false,
                Message = response.Msg
            };
        }

        var user = response.Player;


        // Select all dates from the user
        var dates = user!.Dates
            .Select(d => DateOnly.ParseExact(d.DateFormatted, "dd-MM-yyyy HH:mm", null))
            .ToList();

        var abc = (await _symbolService.GetLogos(dates))
            .SelectMany(s => s.Dates)
            .ToList();

        // Get all symbols for the dates
        var symbols = (await _symbolService.GetLogos(dates))
            .SelectMany(s => s.Dates)
            .ToDictionary(d => d.Date, d => d);

        var totalDates = user.Dates.Count;
        var datoer = new List<Dato>();
        for (var i = totalDates - 1; i >= 0; i--)
        {
            var start = DateTimeOffset.ParseExact(user.Dates[i].DateFormatted, "dd-MM-yyyy HH:mm", null);
            var end = DateTimeOffset.ParseExact(user.Dates[i].EndDateFormatted, "dd-MM-yyyy HH:mm", null);
            var date = DateOnly.FromDateTime(start.Date);

            datoer.Add(new Dato()
            {
                Number = totalDates - i,
                Date = date,
                Symbol = symbols.GetValueOrDefault(date)?.ToDto(),
                Start = start,
                Finish = end,
            });
        }

        var scrapedUser = new UserDto()
        {
            ProfileId = user.Id,
            Name = user.Alias,
            Krus = user.GlassNumber,
            AvatarUrl = user.Photo,
            Dates = datoer
        };

        // Load the user from the database
        var dbUser = await _dbContext.Users
            .Include(u => u.Datoer)
            .Include(u => u.Permissions)
            .FirstOrDefaultAsync(u => u.ProfileId == user.Id);
        if (dbUser is null) dbUser = await _userService.UpsertUser(scrapedUser, dbUser);

        // Update comments
        var dbChanges = false;
        user.Dates
            .Where(d => !string.IsNullOrEmpty(d.Comment))
            .ToList()
            .ForEach(d =>
            {
                var dbDate = dbUser.Datoer.FirstOrDefault(d2 => d2.Date == ParseDate(d.DateFormatted));
                if (dbDate is null) return;
                if (!string.IsNullOrWhiteSpace(dbDate.Comment)) return;
                dbDate.Comment = d.Comment;
                dbChanges = true;
            });
        if (dbChanges) await _dbContext.SaveChangesAsync();

        // Return the final response
        return new LoginResponse
        {
            Success = true,
            Message = response.Msg,
            Token = user.Token,
            Profile = scrapedUser,
            Permissions = dbUser.Permissions.Select(p => p.Name).ToList()
        };
    }

    private static DateOnly ParseDate(string date)
    {
        return DateOnly.ParseExact(date, "dd-MM-yyyy HH:mm", null);
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
        await _httpClient.PostAsync(url, formContent);
    }

    public async Task<StatusResponse> VerifySecret(CodeRequest request)
    {
        var user = _httpSession.GetAuthentication();
        var code = request.Code.ToLower();

        if (user.Permissions == null) user.Permissions = new List<string>();
        if (user.Permissions!.Contains(code))
        {
            return new StatusResponse
            {
                Success = false,
                Message = "Du har allerede indløst denne kode"
            };
        }

        switch (code)
        {
            case Role.BeerPong:
                await AddRoleToUser(user.Profile!.ProfileId, code);
                _authCache.Remove(user.Token!);
                return new StatusResponse
                {
                    Success = true,
                    Message = "Koden er blevet indløst"
                };
        }

        return new StatusResponse
        {
            Success = false,
            Message = "Ugyldig kode"
        };
    }

    private async Task AddRoleToUser(string userId, string roleId)
    {
        var user = await _dbContext.Users
            .Include(u => u.Permissions)
            .FirstOrDefaultAsync(u => u.ProfileId == userId);
        user!.Permissions.Add(new Permission
        {
            Name = roleId,
        });
        await _dbContext.SaveChangesAsync();
    }
}