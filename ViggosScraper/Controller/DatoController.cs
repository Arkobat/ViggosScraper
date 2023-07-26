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

    public DatoController(UserScraper userScraper, SearchScraper searchScraper, HighscoreScraper highscoreScraper,
        SymbolService symbolService)
    {
        _userScraper = userScraper;
        _searchScraper = searchScraper;
        _highscoreScraper = highscoreScraper;
        _symbolService = symbolService;
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
    public async Task<List<SymbolDefinition>> GetSymbols()
    {
        var symbols = _symbolService.GetAll();

        var definitions = new List<SymbolDefinition>();
        foreach (var symbol in symbols)
        {
            var definition = definitions.FirstOrDefault(s => s.Symbol == symbol.Symbol);

            if (definition is not null) definition!.Dates.Add(symbol.Date);
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

        return definitions;
    }
}