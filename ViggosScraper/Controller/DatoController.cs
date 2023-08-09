using Microsoft.AspNetCore.Mvc;
using ViggosScraper.Model;
using ViggosScraper.Service;

namespace ViggosScraper.Controller;

public class DatoController : ControllerBase
{
    private readonly UserScraper _userScraper;
    private readonly SearchScraper _searchScraper;
    private readonly HighscoreScraper _highscoreScraper;

    public DatoController(UserScraper userScraper, SearchScraper searchScraper, HighscoreScraper highscoreScraper)
    {
        _userScraper = userScraper;
        _searchScraper = searchScraper;
        _highscoreScraper = highscoreScraper;
    }

    [HttpGet("search/{searchTerm}")]
    public async Task<List<SearchResult>> SearchUser(string searchTerm)
    {
        return await _searchScraper.Search(searchTerm);
    }

    [HttpGet("profile/{profileId}")]
    public async Task<UserDto> GetUser(string profileId)
    {
        return await _userScraper.GetUser(profileId);
    }

    [HttpGet("highscore")]
    public async Task<List<HighscoreEntry>> GetHighscore()
    {
        return await _highscoreScraper.GetHighscore(null);
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
}

