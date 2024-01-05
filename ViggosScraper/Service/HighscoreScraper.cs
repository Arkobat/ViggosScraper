using HtmlAgilityPack;
using ViggosScraper.Extension;
using ViggosScraper.Model;
using ViggosScraper.Model.Response;

namespace ViggosScraper.Service;

public class HighscoreScraper
{
    private readonly ILogger<HighscoreScraper> _logger;

    public HighscoreScraper(ILogger<HighscoreScraper> logger)
    {
        _logger = logger;
    }

    public async Task<List<HighscoreEntry>> GetHighscore(int? year)
    {
        var web = new HtmlWeb();

        var url = "https://www.drikdato.dk/ViggosOdense/Statistik";
        if (year is null) url += "/All";
        else url += $"/{year}";

        var htmlDoc = await web.LoadFromWebAsync(url);

        if (year is null)
        {
            _logger.LogInformation("Scraping all time highscore");
            return AllTime(htmlDoc);
        }

        _logger.LogInformation("Scraping highscore for year {Year}", year);
        return SingleYear(htmlDoc);
    }

    private List<HighscoreEntry> AllTime(HtmlDocument htmlDoc)
    {
        var nodesCollection = htmlDoc.DocumentNode.SelectNodes(
            "/" +
            "/div[@class='floatPod']" +
            "/div[@class='padding']" +
            "/table" +
            "/tr"
        );

        if (nodesCollection is null)
        {
            _logger.LogWarning("Could not find any highscore entries");
            return new List<HighscoreEntry>();
        }

        return nodesCollection
            .Where(n => n.Attributes["onclick"] != null)
            .Select(n => ParseEntry(n.InnerHtml, SearchScraper.ExtractUserId(n)))
            .ToList();
    }

    private List<HighscoreEntry> SingleYear(HtmlDocument htmlDoc)
    {
        var nodesCollection = htmlDoc.DocumentNode.SelectNodes(
            "/" +
            "/div[@class='floatPod']" +
            "/div[@class='padding']" +
            "/table" +
            "/tr"
        );

        if (nodesCollection is null)
        {
            _logger.LogWarning("Could not find any highscore entries");
            return new List<HighscoreEntry>();
        }

        return nodesCollection
            .Where(n => n.Attributes["onclick"]?.Value.Contains("window.location.href='/ViggosOdense/Profil/") ?? false)
            .Select(n => ParseEntry(n.InnerHtml, SearchScraper.ExtractUserId(n)))
            .ToList();
    }

    private HighscoreEntry ParseEntry(string html, string userId)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        var nodes = htmlDoc.DocumentNode
            .SelectNodes("//td")
            .ToArray();

        return new HighscoreEntry()
        {
            Position = int.Parse(nodes[0].InnerText.DigitOnly()),
            Name = nodes[1].InnerText,
            ProfileId = userId,
            TotalDates = int.Parse(nodes[2].InnerText.DigitOnly())
        };
    }
}