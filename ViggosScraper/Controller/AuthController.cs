﻿using System.Net;
using Microsoft.AspNetCore.Mvc;
using ViggosScraper.Model;
using ViggosScraper.Service;

namespace ViggosScraper.Controller;

public class AuthController : ControllerBase
{
    private readonly LoginService _loginService;

    public AuthController(LoginService loginService)
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
}