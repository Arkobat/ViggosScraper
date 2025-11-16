using DrikDatoApp.Model;
using DrikDatoApp.Service;
using Microsoft.AspNetCore.Mvc;
using ViggosScraper.Model.Request;
using ViggosScraper.Model.Response;
using ViggosScraper.Service;
using UserDto = ViggosScraper.Model.Response.UserDto;

namespace ViggosScraper.Controller;

public class LoginController(
    IDrikDatoService drikDatoService,
    LoginService loginService,
    SymbolService symbolService
) : ControllerBase
{
    [HttpPost("login")]
    public async Task<LoginResponse> Login([FromBody] LoginRequest request)
    {
        var result = await loginService.Login(request.Username, request.Password);
        
        var symbols = await symbolService.GetLogos(
            result.User.Datoer.Select(d => d.Date).ToList(),
            result.User.Permissions.Select(p => p.Name).ToList()
        );
        
        return new LoginResponse
        {
            Token = result.SelfUser.Token,
            Profile = new UserDto
            {
                ProfileId = result.User.ProfileId.ToString(),
                Name = result.User.Name,
                AvatarUrl = result.User.AvatarUrl,
                Krus = result.User.Glass,
                Dates = result.User.Datoer.Select(d => new Dato
                {
                    Number = d.Number,
                    Date = d.Date,
                    Symbol = SymbolService.GetSymbol(symbols, d.Date, d.StartDate),
                    Start = d.StartDate,
                    Finish = d.EndDate
                }).ToList(),
            },
            Permissions = result.User.Permissions.Count == 0
                ? null
                : result.User.Permissions.Select(p => p.Name).ToList(),
            Success = true,
            Message = string.Empty,
        };
    }

    [HttpPost("authenticate")]
    public async Task<LoginResponse> Authenticate([FromBody] AuthRequest request)
    {
        var result = await loginService.ValidateToken(request.Token);

        var symbols = await symbolService.GetLogos(
            result.User.Datoer.Select(d => d.Date).ToList(),
            result.User.Permissions.Select(p => p.Name).ToList()
        );
        
        return new LoginResponse
        {
            Token = result.SelfUser.Token,
            Profile = new UserDto
            {
                ProfileId = result.User.ProfileId.ToString(),
                Name = result.User.Name,
                AvatarUrl = result.User.AvatarUrl,
                Krus = result.User.Glass,
                Dates = result.User.Datoer.Select(d => new Dato
                {
                    Number = d.Number,
                    Date = d.Date,
                    Symbol = SymbolService.GetSymbol(symbols, d.Date, d.StartDate),
                    Start = d.StartDate,
                    Finish = d.EndDate
                }).ToList(),
            },
            Permissions = result.User.Permissions.Select(p => p.Name).ToList(),
            Success = true,
            Message = string.Empty,
        };
    }

    [HttpPost("reset-password")]
    public async Task ResetPassword([FromBody] ResetPasswordRequest request)
    {
        await drikDatoService.ResetPassword(request.PhoneNumber);
    }

    [HttpPost("verify-secret")]
    public Task<StatusResponse> VerifySecret([FromBody] CodeRequest request)
    {
        throw new NotImplementedException();
    }

    [HttpPost("avatar")]
    [RequestSizeLimit(10_000_000)] // 10 MB
    public async Task<AvatarChangeResponse> SetAvatar(IFormFile file)
    {
        var token = Request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrEmpty(token))
        {
            throw new UnauthorizedAccessException("Missing authorization token.");
        }
        
        using var memoryStream = new MemoryStream();
        await file.OpenReadStream().CopyToAsync(memoryStream);
        
        return await loginService.ChangeAvatar(token, memoryStream);
    }
    
}