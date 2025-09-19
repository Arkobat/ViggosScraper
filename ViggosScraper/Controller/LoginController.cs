using DrikDatoApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViggosScraper.Model.Request;
using ViggosScraper.Model.Response;
using ViggosScraper.Service;

namespace ViggosScraper.Controller;

public class LoginController(
    IDrikDatoService drikDatoService,
    LoginService loginService
) : ControllerBase
{
    [HttpPost("login")]
    public async Task<LoginResponse> Login([FromBody] LoginRequest request)
    {
        var result = await loginService.Login(request.Username, request.Password);
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
                    Symbol = null,
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
                    Symbol = null,
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

    [Authorize]
    [HttpPost("verify-secret")]
    public Task<StatusResponse> VerifySecret([FromBody] CodeRequest request)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpPost("avatar")]
    [RequestSizeLimit(10_000_000)] // 10 MB
    public Task<StatusResponse> SetAvatar(IFormFile file)
    {
        throw new NotImplementedException();
    }
}