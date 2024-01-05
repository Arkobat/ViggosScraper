using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using ViggosScraper.Model;
using ViggosScraper.Model.Response;
using ViggosScraper.Service;

namespace ViggosScraper.Middleware;

public class BasicAuthenticationOptions : AuthenticationSchemeOptions
{
}

public class CustomAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
{
    private readonly ICache<string, AuthResponse> _authCache;
    private readonly LoginService _loginService;
    private readonly HttpSession _httpSession;
    private readonly ILogger<CustomAuthenticationHandler> _logger;

    public CustomAuthenticationHandler(IOptionsMonitor<BasicAuthenticationOptions> options, ILoggerFactory logger,
        UrlEncoder encoder, ISystemClock clock, ICache<string, AuthResponse> authCache, LoginService loginService, HttpSession httpSession) :
        base(options, logger, encoder, clock)
    {
        _authCache = authCache;
        _loginService = loginService;
        _httpSession = httpSession;
        _logger = logger.CreateLogger<CustomAuthenticationHandler>();
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var header = Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(header))
        {
            _logger.LogDebug("No Authorization header found");
            return Fail().AuthenticateResult;
        }

        _logger.LogDebug("Trying to validate auth token {Header}", header);
        var auth = await _authCache.Get(header, async () => await ValidateToken(header), TimeSpan.FromMinutes(5));
        if (auth.AuthenticateResult.Succeeded) _httpSession.Initialize(auth.LoginResponse!);

        return auth.AuthenticateResult;
    }

    private static AuthResponse Fail(string reason = "Unauthorized")
    {
        return new AuthResponse(AuthenticateResult.Fail(reason));
    }

    private async Task<AuthResponse> ValidateToken(string token)
    {
        var loginResponse = await _loginService.Authenticate(token);
        if (!loginResponse.Success)
        {
            _logger.LogDebug("Unable to validate token");
            return Fail();
        }

        var identity = new ClaimsIdentity(new List<Claim>(), Scheme.Name);
        var principal = new GenericPrincipal(identity, loginResponse.Permissions?.ToArray());
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return new AuthResponse(AuthenticateResult.Success(ticket), loginResponse);
    }

}

public class AuthResponse
{
    public AuthenticateResult AuthenticateResult { get; set; }
    public LoginResponse? LoginResponse { get; set; }

    public AuthResponse(AuthenticateResult authenticateResult, LoginResponse? response = null)
    {
        AuthenticateResult = authenticateResult;
        LoginResponse = response;
    }
}