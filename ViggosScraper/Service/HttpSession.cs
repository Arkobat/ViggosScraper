using ViggosScraper.Model.Response;

namespace ViggosScraper.Service;

public class HttpSession
{
    private LoginResponse? _authDto;
    private bool _initialized;

    public void Initialize(LoginResponse authDto)
    {
        if (authDto is null) throw new ArgumentNullException(nameof(authDto));
        _authDto = authDto;
        _initialized = true;
    }

    public LoginResponse GetAuthentication()
    {
        if (!_initialized) throw new InvalidOperationException("HttpContext not initialized");
        return _authDto!;
    }

    private LoginResponse GetAuth()
    {
        if (!_initialized) throw new InvalidOperationException("HttpContext not initialized");
        return _authDto!;
    }

    public string GetUserId()
    {
        var auth = GetAuth();
        return auth.Profile!.ProfileId;
    }

}