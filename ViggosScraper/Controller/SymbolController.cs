using Microsoft.AspNetCore.Mvc;
using ViggosScraper.Service;

namespace ViggosScraper.Controller;

[Microsoft.AspNetCore.Components.Route("symbol")]
public class SymbolController(
#pragma warning disable CS9113 // Parameter is unread.
    SymbolService symbolService
#pragma warning restore CS9113 // Parameter is unread.
) : ControllerBase
{
    //[HttpPost("seed")]
    //public async Task Seed()
    //{
    //    await symbolService.Seed();
    //}
}