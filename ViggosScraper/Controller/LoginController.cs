using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViggosScraper.Model;
using ViggosScraper.Model.Request;
using ViggosScraper.Model.Response;
using ViggosScraper.Service;

namespace ViggosScraper.Controller;

public class LoginController : ControllerBase
{
    private readonly LoginService _loginService;

    public LoginController(LoginService loginService)
    {
        _loginService = loginService;
    }

    [HttpPost("login")]
    public async Task<LoginResponse> Login([FromBody] LoginRequest request)
    {
        var result = await _loginService.Login(request.Username, request.Password);
        if (!result.Success) Response.StatusCode = (int) HttpStatusCode.Unauthorized;
        return result;
    }

    [HttpPost("authenticate")]
    public async Task<LoginResponse> Authenticate([FromBody] AuthRequest request)
    {
        var result = await _loginService.Authenticate(request.Token);
        if (!result.Success) Response.StatusCode = (int) HttpStatusCode.Unauthorized;

        return result;
    }

    [HttpPost("reset-password")]
    public async Task ResetPassword([FromBody] ResetPasswordRequest request)
    {
        await _loginService.ResetPassword(request.PhoneNumber);
    }

    [Authorize]
    [HttpPost("verify-secret")]
    public async Task<StatusResponse> VerifySecret([FromBody] CodeRequest request)
    {
        return await _loginService.VerifySecret(request);
    }

    [Authorize]
    [HttpPost("avatar")]
    [RequestSizeLimit(10_000_000)] // 10 MB
    public async Task<StatusResponse> SetAvatar(IFormFile file)
    {
        return await _loginService.SetAvatar(file);
    }
}