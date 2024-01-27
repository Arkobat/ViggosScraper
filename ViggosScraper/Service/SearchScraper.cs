using HtmlAgilityPack;
using ViggosScraper.Extension;
using ViggosScraper.Model.Response;

namespace ViggosScraper.Service;

public class SearchScraper
{

    private readonly ILogger<SearchScraper> _logger;

    public SearchScraper(ILogger<SearchScraper> logger)
    {
        _logger = logger;
    }

    public async Task<List<SearchResult>> Search(string searchQuery)
    {
        var url = $"https://www.drikdato.dk/ViggosOdense/Sogning/{searchQuery}";
        _logger.LogInformation("Searching for {searchQuery}", searchQuery);

        var web = new HtmlWeb();
        var htmlDoc = await web.LoadFromWebAsync(url);
        
        var nodes = htmlDoc.DocumentNode.SelectNodes(
            "/" +
            "/div[@class='floatPod']" +
            "/div[@class='padding']" +
            "/table" +
            "/tr"
        );

        var results = nodes?
            .Where(n => !n.InnerText.StartsWith("\\"))
            .Where(n => n.Attributes["onclick"] != null)
            .Select(n => new SearchResult()
            {
                Name = n.InnerText.Trim(),
                ProfileId = ExtractUserId(n)
            }).ToList() ?? new List<SearchResult>();

        _logger.LogInformation("Found {count} results", results.Count);

        return results;
    }

    public static string ExtractUserId(HtmlNode node)
    {
        return node.Attributes["onclick"].Value
            .Split("/").Last()
            .DigitOnly();
    }
}