using Microsoft.AspNetCore.Mvc;
using ViggosScraper.Service;

namespace ViggosScraper.Controller;

[Route("symbols")]
public class LogoController : ControllerBase
{
    private readonly SymbolService _symbolService;

    public LogoController(SymbolService symbolService)
    {
        _symbolService = symbolService;
    }

    [HttpGet]
    public async Task GetAllSymbols()
    {
        throw new NotImplementedException();
    }


    [HttpPost("seed")]
    public async Task Seed()
    {
        await _symbolService.Seed();
    }
}