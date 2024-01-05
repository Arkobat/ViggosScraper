using Microsoft.EntityFrameworkCore;
using ViggosScraper.Database;
using ViggosScraper.Model;
using ViggosScraper.Model.Exception;
using ViggosScraper.Model.Request;
using ViggosScraper.Model.Response;

namespace ViggosScraper.Service;

public class BeerPongService
{
    private readonly ViggosDb _dbContext;
    private readonly UserService _userService;
    private readonly HttpSession _httpSession;

    public BeerPongService(ViggosDb dbContext, HttpSession httpSession, UserService userService)
    {
        _dbContext = dbContext;
        _httpSession = httpSession;
        this._userService = userService;
    }

    public async Task AddBattle(CreateBattleDto request)
    {
        var userId = _httpSession.GetUserId();
        if (!request.Winners.Contains(userId) && !request.Losers.Contains(userId))
            throw new BadRequestException("You must be a part of the battle");

        if ( request.Winners.Any(i => request.Losers.Contains(i))) 
            throw new BadRequestException("A user cannot be on both teams");
        
        var battle = new BeerPongBattle()
        {
            Time = DateTimeOffset.UtcNow,
            Confirmed = false,
            Results = new List<BattleResult>()
        };

        foreach (var winner in request.Winners)
        {
            var user = await _userService.GetUser(winner);
            if (!user.HasPermission(Role.BeerPong)) throw new BadRequestException("All users must have access");
            battle.Results.Add(new BattleResult()
            {
                Won = true,
                User = user,
            });
        }
        
        foreach (var loser in request.Losers)
        {
            var user = await _userService.GetUser(loser);
            if (!user.HasPermission(Role.BeerPong)) throw new BadRequestException("All users must have access");
            battle.Results.Add(new BattleResult()
            {
                Won = false,
                User = user,
            });
        }

        _dbContext.BeerPongBattles.Add(battle);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<HighscoreEntry>> GetAllPersons()
    {
        var users = await _dbContext.Users
            .Where(u => u.Permissions.Any(p => p.Name == Role.BeerPong))
            .Include(u => u.Battles).ThenInclude(r => r.Battle)
            .ToListAsync();

        var ordered = users.Select(u => new
        {
            User = u,
            Points = u.Battles.Count(b => b.Won) - u.Battles.Count(b => !b.Won)
        }).OrderByDescending(u => u.Points);

        var index = 1;
        return ordered.Select(u => new HighscoreEntry
        {
            Position = index++,
            Name = u.User.Name,
            ProfileId = u.User.ProfileId,
            TotalDates = u.Points
        }).ToList();
    }

    public async Task<List<BeerPongBattle>> GetAllBattles(int page)
    {
        const int total = 500;
        var start = (page - 1) * total;
        return await _dbContext.BeerPongBattles
            .Include(b => b.Results).ThenInclude(r => r.User)
            .OrderByDescending(b => b.Time)
            .Skip(start).Take(total)
            .ToListAsync();
    }

    public async Task ConfirmBattle(int battleId)
    {
        var battle = await _dbContext.BeerPongBattles
            .Include(b => b.Results).ThenInclude(r => r.User)
            .FirstOrDefaultAsync(b => b.BattleId == battleId);
        if (battle is null) throw new NotFoundException($"Cannot find any battles with id {battleId}");

        var userId = _httpSession.GetUserId();
        if (battle.Results.Any(b => b.User.ProfileId == userId))
            throw new BadRequestException("You cannot confirm your own battles");

        battle.Confirmed = true;
        await _dbContext.SaveChangesAsync();
    }
}