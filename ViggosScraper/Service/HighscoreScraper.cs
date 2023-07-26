using HtmlAgilityPack;
using ViggosScraper.Extension;
using ViggosScraper.Model;

namespace ViggosScraper.Service;

public class HighscoreScraper
{

    public async Task<List<HighscoreEntry>> GetHighscore(int? year)
    {
        var web = new HtmlWeb();
        if (year is null)
        {
            const string url = "https://www.drikdato.dk/ViggosOdense/Statistik/All";
            var htmlDoc = await web.LoadFromWebAsync(url);
            return AllTime(htmlDoc);
        }

        throw new NotImplementedException();
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
            Console.WriteLine("No nodes found");
            return new List<HighscoreEntry>();
        }

        return nodesCollection
            .Where(n => n.Attributes["onclick"] != null)
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