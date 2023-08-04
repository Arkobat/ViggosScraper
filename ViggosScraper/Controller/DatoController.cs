using System.Net;
using Microsoft.AspNetCore.Mvc;
using ViggosScraper.Model;
using ViggosScraper.Service;

namespace ViggosScraper.Controller;

public class DatoController : ControllerBase
{
    private readonly UserScraper _userScraper;
    private readonly SearchScraper _searchScraper;
    private readonly HighscoreScraper _highscoreScraper;
    private readonly SymbolService _symbolService;
    private readonly LoginService _loginService;

    public DatoController(UserScraper userScraper, SearchScraper searchScraper, HighscoreScraper highscoreScraper,
        SymbolService symbolService, LoginService loginService)
    {
        _userScraper = userScraper;
        _searchScraper = searchScraper;
        _highscoreScraper = highscoreScraper;
        _symbolService = symbolService;
        _loginService = loginService;
    }

    [HttpGet("search/{searchTerm}")]
    public async Task<List<SearchResult>> SearchUser(string searchTerm)
    {
        return await _searchScraper.Search(searchTerm);
    }

    [HttpGet("profile/{profileId}")]
    public async Task<User> GetUser(string profileId)
    {
        return await _userScraper.GetUser(profileId);
    }

    [HttpGet("highscore")]
    public async Task<List<HighscoreEntry>> GetHighscore()
    {
        return await _highscoreScraper.GetHighscore(null);
    }

    [HttpGet("symbols")]
    public Task<List<SymbolDefinition>> GetSymbols()
    {
        var symbols = _symbolService.GetAll();

        var definitions = new List<SymbolDefinition>();
        foreach (var symbol in symbols)
        {
            var definition = definitions.FirstOrDefault(s => s.Symbol == symbol.Symbol);

            if (definition is not null) definition.Dates.Add(symbol.Date);
            else
            {
                definition = new SymbolDefinition()
                {
                    Symbol = symbol.Symbol,
                    Description = symbol.Reason,
                    Dates = new List<DateOnly>() {symbol.Date}
                };
                definitions.Add(definition);
            }
        }

        return Task.FromResult(definitions);
    }

    [HttpGet("mutual/{user1}/{user2}")]
    public async Task<List<Dato>> GetMutual(string user1, string user2)
    {
        var result1 = await _userScraper.GetUser(user1);
        var result2  = await _userScraper.GetUser(user2);

        return result1.Dates
            .Where(r1 => result2.Dates.Any(r2 => r1.Date == r2.Date))
            .ToList();
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