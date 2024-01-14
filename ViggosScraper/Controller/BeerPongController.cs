using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViggosScraper.Model;
using ViggosScraper.Model.Request;
using ViggosScraper.Model.Response;
using ViggosScraper.Service;

namespace ViggosScraper.Controller;

[Route("beerpong")]
[Authorize(Roles = Role.BeerPong)]
public class BeerPongController : ControllerBase
{
    private readonly BeerPongService _beerPongService;

    public BeerPongController(BeerPongService beerPongService)
    {
        _beerPongService = beerPongService;
    }

    [HttpGet("users")]
    public async Task<List<BeerpongHighscoreEntry>> GetAllUsers()
    {
        return await _beerPongService.GetAllPersons();
    }

    [HttpGet("battles")]
    public async Task<List<BattleDto>> GetAllBattles([FromQuery] int page = 1)
    { 
        return (await _beerPongService.GetAllBattles(page))
            .Select(b => new BattleDto()
            {
                BattleId = b.BattleId,
                Confirmed = b.Confirmed,
                Time = b.Time,
                Winners = b.Results.Where(r => r.Won).Select(r => r.User.ProfileId).ToList(),
                Losers = b.Results.Where(r => !r.Won).Select(r => r.User.ProfileId).ToList()
            }).ToList();
    }
    
    [HttpPost("battle")]
    public async Task AddBattle([FromBody] CreateBattleDto request)
    {
        await _beerPongService.AddBattle(request);
    }
        
    [HttpPost("confirm/{battleId:int}")]
    public async Task AddBattle([FromRoute] int battleId)
    {
        await _beerPongService.ConfirmBattle(battleId);
    }
}