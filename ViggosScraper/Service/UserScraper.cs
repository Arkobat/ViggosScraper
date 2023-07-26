﻿using System.Net;
using HtmlAgilityPack;
using ViggosScraper.Middleware;
using ViggosScraper.Model;

namespace ViggosScraper.Service;

public class UserScraper
{
    private ILogger<UserScraper> _logger;
    private readonly SymbolService _symbolService;

    public UserScraper(ILogger<UserScraper> logger, SymbolService symbolService)
    {
        _logger = logger;
        _symbolService = symbolService;
    }

    public async Task<User> GetUser(string userId)
    {
        _logger.LogInformation("Getting user {userId}", userId);
        
        var url = $"https://www.drikdato.dk/ViggosOdense/Profil/{userId}";

        var web = new HtmlWeb();
        var htmlDoc = await web.LoadFromWebAsync(url);

        if (htmlDoc.DocumentNode.InnerText.Contains("Profilen blev ikke fundet..."))
        {
            _logger.LogInformation("User {userId} not found", userId);
            throw new HttpException(HttpStatusCode.NotFound, $"Could not find any user with that id {userId}");
        }
        
        var userInfo = htmlDoc.DocumentNode.SelectNodes(
                "/" +
                "/div[@class='floatPod']" +
                "/div[@class='padding']" +
                "/table" +
                "/tr" +
                "/td" +
                "/div[@class='headline']"
            )
            .Select(n => n.InnerText.Trim())
            .ToArray();

        var avatar = htmlDoc.DocumentNode.SelectNodes(
            "/" +
            "/div[@class='floatPod']" +
            "/div[@class='padding']" +
            "/table" +
            "/tr" +
            "/td" +
            "/img/@src"
        ).Single().Attributes["src"].Value;

        var user = new User()
        {
            ProfileId = userId,
            Name = userInfo[0],
            AvatarUrl = $"https://www.drikdato.dk{avatar}",
            Krus = userInfo[1],
            Dates = GetDates(htmlDoc)
        };

        return user;
    }

    private List<Dato> GetDates(HtmlDocument htmlDoc)
    {
        var nodes = htmlDoc.DocumentNode.SelectNodes(
            "/" +
            "/div[@class='floatPod']" +
            "/div[@class='padding']" +
            "/div[@class='yearlayer']" +
            "/table" +
            "/tr"
        );

        if (nodes == null)
            return new List<Dato>();

        var dates = nodes
            .Where(n => n.InnerText != "Nr.Dato")
            .Select(n => n.InnerText
                .Replace("\t", "")
                .Split("\r\n")
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray())
            .Select(s => ParseDate(s[1]))
            .ToList();

        var symbols = _symbolService.GetSymbols(dates);
        return dates
            .Order()
            .Select((date, i) => new Dato
            {
                Number = i + 1,
                Date = date,
                Symbol = symbols.FirstOrDefault(s => s.Date == date)?.Simple()
            }).ToList();
    }

    private static DateOnly ParseDate(string date)
    {
        var data = date.Replace(".", "").Split(" ");

        var day = int.Parse(data[0]);
        var year = int.Parse(data[2]);

        return data[1] switch
        {
            "januar" => new DateOnly(year, 1, day),
            "februar" => new DateOnly(year, 2, day),
            "marts" => new DateOnly(year, 3, day),
            "april" => new DateOnly(year, 4, day),
            "maj" => new DateOnly(year, 5, day),
            "juni" => new DateOnly(year, 6, day),
            "juli" => new DateOnly(year, 7, day),
            "august" => new DateOnly(year, 8, day),
            "september" => new DateOnly(year, 9, day),
            "oktober" => new DateOnly(year, 10, day),
            "november" => new DateOnly(year, 11, day),
            "december" => new DateOnly(year, 12, day),
            _ => throw new ArgumentOutOfRangeException(nameof(date), date, null)
        };
    }
}