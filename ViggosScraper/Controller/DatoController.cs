using Microsoft.AspNetCore.Mvc;
using ViggosScraper.Model;
using ViggosScraper.Model.Response;
using ViggosScraper.Service;

namespace ViggosScraper.Controller;

public class DatoController : ControllerBase
{
    private readonly UserService _userService;
    private readonly UserScraper _userScraper;
    private readonly SearchScraper _searchScraper;
    private readonly HighscoreScraper _highscoreScraper;

    public DatoController(UserScraper userScraper, SearchScraper searchScraper, HighscoreScraper highscoreScraper,
        UserService userService)
    {
        _userScraper = userScraper;
        _searchScraper = searchScraper;
        _highscoreScraper = highscoreScraper;
        _userService = userService;
    }

    [HttpGet("search/{searchTerm}")]
    public async Task<List<SearchResult>> SearchUser(string searchTerm)
    {
        return await _searchScraper.Search(searchTerm);
    }

    [HttpGet("profile/{profileId}")]
    public async Task<UserDto> GetUser(string profileId)
    {
        var dbUser = await _userService.GetUser(profileId);
        var userDto = await _userScraper.GetUser(dbUser, profileId);
        return userDto;
    }

    [HttpGet("highscore")]
    public async Task<List<HighscoreEntry>> GetHighscore([FromQuery] int? year)
    {
        return await _highscoreScraper.GetHighscore(year);
    }
}