using HtmlAgilityPack;

// TODO: Get keywords from references
// TODO: Add synonyms of keywords to keywords-mapping
class Scraper
{
    private readonly Uri _url;
    private HtmlDocument html;
    private string _articleName;

    private List<string> links = new List<string>();

    private List<string> _categories;

    private List<KeyWord> _keywords = new List<KeyWord>();

    public Scraper(Uri url)
    {
        _url = url;
    }

    public string GetArticleName()
    {
        return _articleName;
    }

    public List<string> GetCategories()
    {
        return _categories;
    }

    public List<KeyWord> GetKeywords()
    {
        return _keywords;
    }

    public async Task Scrape()
    {
        // get HTML as string
        string htmlString = await GetHtml();

        // parse into HtmlDocument
        var doc = new HtmlDocument();
        doc.LoadHtml(htmlString);
        html = doc;

        ParseArticleName();

        //ParseCategories();
        ParseKeywords();
    }

    private async Task<string> GetHtml()
    {
        using var http = new HttpClient();
        http.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Ronkle/1.0 (Crawling and indexing wikipedia pages to build a wiki-search engine for fun, private use only)"
        );

        var response = await http.GetAsync(_url);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Request failed: {(int)response.StatusCode} {response.ReasonPhrase}");
        }

        return await response.Content.ReadAsStringAsync();
    }

    private void ParseArticleName()
    {
        try
        {
            _articleName = html.DocumentNode.SelectSingleNode("//h1[@id='firstHeading']").InnerText.Trim();
        }
        catch
        {
            throw new Exception("could not parse title");
        }
    }

    private void ParseCategories()
    {
        try
        {
            var categoryDiv = html.DocumentNode.SelectSingleNode("//div[@id='mw-normal-catlinks']");
            var categoryLinks = categoryDiv
                .SelectNodes(".//a[not(@title='Help:Category')]")
                ?.Select(a => a.InnerText.Trim())
                .ToList();

            _categories = categoryLinks;
        }
        catch
        {
            throw new Exception("could not parse categories");
        }
    }

    public List<string> GetLinks()
    {
        return links;
    }

    private void ParseKeywords()
    {
        var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "read",
            "article",
            "edit",
            "help",
            "citation needed",
            "main page"
        };

        try
        {
            var allLinks = html.DocumentNode.SelectNodes("//div[@id='bodyContent']//a[@href]") ?? Enumerable.Empty<HtmlNode>();

            /*var filteredKeywords = allLinks
                // exclude links with certain classes
                .Where(a => !a.GetAttributeValue("class", "").Split(' ').Contains("external"))
                // exclude links inside certain IDs
                .Where(a => a.Ancestors()
                            .All(p => p.GetAttributeValue("id", "") != "catlinks" &&
                                    p.GetAttributeValue("id", "") != "footer"))
                // only internal /wiki/ links, no special pages
                .Where(a =>
                {
                    var href = a.GetAttributeValue("href", "");
                    return href.StartsWith("/wiki/") && !href.Contains(":");
                })
                // only keep links with visible text
                .Select(a => a.InnerText.Trim())
                .Where(text => !string.IsNullOrWhiteSpace(text))
                .Where(text => !stopWords.Any(word => text.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0));*/
            var filteredKeywords = linkNodes
                .Where(a =>
                {
                    var href = a.GetAttributeValue("href", "");
                    return href.StartsWith("/wiki/") && !href.Contains(":");
                })
                .Select(a => a.InnerText.Trim())
                .Where(text =>
                    !string.IsNullOrWhiteSpace(text) &&
                    text.Length > 2 &&
                    !stopWords.Contains(text.ToLower())
                );

            var filteredLinks = allLinks
                .Select(a => a.GetAttributeValue("href", ""))
                .Where(href => href.StartsWith("/wiki/") && !href.Contains(":"))
                .Select(href => "https://en.wikipedia.org" + href)
                .Distinct();

            foreach (var link in filteredLinks)
            {
                links.Add(link);
            }

            var keywordCounts = filteredKeywords
                .GroupBy(text => text)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var kv in keywordCounts)
            {
                _keywords.Add(
                    new KeyWord(kv.Key, kv.Value)
                );
            }

        }
        catch (Exception ex)
        {
            throw new Exception("Could not parse keywords", ex);
        }
    }
}