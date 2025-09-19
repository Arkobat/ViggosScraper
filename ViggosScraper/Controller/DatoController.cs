using DrikDatoApp.Service;
using Microsoft.AspNetCore.Mvc;
using ViggosScraper.Model.Exception;
using ViggosScraper.Model.Response;
using ViggosScraper.Service;

namespace ViggosScraper.Controller;

public class DatoController(
    IDrikDatoService drikDatoService,
    NewUserService userService,
    UserScraper userScraper
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
                Symbol = null,
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