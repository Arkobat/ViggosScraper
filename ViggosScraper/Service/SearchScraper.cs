using HtmlAgilityPack;
using ViggosScraper.Extension;
using ViggosScraper.Model;

namespace ViggosScraper.Service;

public class SearchScraper
{
    public async Task<List<SearchResult>> Search(string searchQuery)
    {
        var url = $"https://www.drikdato.dk/ViggosOdense/Sogning/{searchQuery}";

        var web = new HtmlWeb();
        var htmlDoc = await web.LoadFromWebAsync(url);

        var nodes = htmlDoc.DocumentNode.SelectNodes(
            "/" +
            "/div[@class='floatPod']" +
            "/div[@class='padding']" +
            "/table" +
            "/tr"
        );

        var results = nodes
            .Where(n => !n.InnerText.StartsWith("\\"))
            .Where(n => n.Attributes["onclick"] != null)
            .Select(n => new SearchResult()
            {
                Name = n.InnerText.Trim(),
                ProfileId = ExtractUserId(n)
            }).ToList();

        foreach (var res in results)
        {
            Console.WriteLine($"{res.Name}: {res.ProfileId}");
        }

        return results;
    }

    public static string ExtractUserId(HtmlNode node)
    {
        return node.Attributes["onclick"].Value
            .Split("/").Last()
            .DigitOnly();
    }
}