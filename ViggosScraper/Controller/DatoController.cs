using DrikDatoApp.Service;
using Microsoft.AspNetCore.Mvc;
using ViggosScraper.Database;
using ViggosScraper.Model.Exception;
using ViggosScraper.Model.Response;
using ViggosScraper.Service;

namespace ViggosScraper.Controller;

public class DatoController(
    IDrikDatoService drikDatoService,
    UserService userService,
    UserScraper userScraper,
    SymbolService symbolService
) : ControllerBase
{
    [HttpGet("search/{searchTerm}")]
    public async Task<List<SearchResult>> SearchUser(string searchTerm)
    {
        var viggosSearch = await drikDatoService.Search(searchTerm);
        var dbSearch = await userService.SearchUsers(searchTerm);

        return viggosSearch.Results.Select(r => new SearchResult
                {
                    Name = r.Alias,
                    ProfileId = r.Id
                }
            )
            .Concat(dbSearch)
            .DistinctBy(d => d.ProfileId)
            .ToList();
    }

    [HttpGet("profile/{profileId:int}")]
    public async Task<UserDto> GetUser(int profileId)
    {
        var user = await userScraper.ScrapeUser(profileId);
        if (user is null)
        {
            throw new NotFoundException($"User with ID {profileId} not found.");
        }

        var symbols = await symbolService.GetLogos(
            user.Datoer.Select(d => d.Date).ToList(),
            user.Permissions.Select(p => p.Name).ToList()
        );

        return new UserDto
        {
            ProfileId = user.ProfileId.ToString(),
            Name = user.Name,
            AvatarUrl = user.AvatarUrl,
            Krus = user.Glass,
            Dates = user.Datoer.Select(d => new Dato
            {
                Number = d.Number,
                Date = d.Date,
                Symbol = SymbolService.GetSymbol(symbols, d.Date, d.StartDate),
                Start = d.EndDate is null ? null : d.StartDate,
                Finish = d.EndDate
            }).ToList(),
        };
    }

    [HttpGet("highscore")]
    public async Task<List<HighscoreEntry>> GetHighscore([FromQuery] int? year)
    {
        var highscore = await userService.Highscore(year);

        return highscore;
    }
}