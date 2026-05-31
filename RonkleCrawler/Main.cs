Queue<string> queue = new();
HashSet<string> visited = new();

queue.Enqueue("https://en.wikipedia.org/wiki/Rick_Worthy");

DBHandler dbHandler = new DBHandler();

await Crawl(500);

async Task Crawl(int maxPages)
{
    Dictionary<string, int> keywordCache = new();

    int pageCount = 0;
    while (queue.Count > 0 && pageCount < maxPages)
    {
        string url = queue.Dequeue();

        if (visited.Contains(url))
            continue;

        visited.Add(url);

        var scraper = new Scraper(new Uri(url));
        await scraper.Scrape();

        var links = scraper.GetLinks();

        foreach (var link in links)
        {
            if (!visited.Contains(link) && !queue.Contains(link))
                queue.Enqueue(link);
        }

        dbHandler.InsertArticle(url, scraper.GetArticleName());
        int artId = dbHandler.GetArticleId(url);

        foreach (KeyWord kw in scraper.GetKeywords())
        {
            string word = kw.GetWord().ToLower();

            if (!keywordCache.TryGetValue(word, out int keyId))
            {
                if (!dbHandler.KeywordExists(word))
                    dbHandler.InsertKeyword(word);

                keyId = dbHandler.GetKeywordId(word);
                keywordCache[word] = keyId;
            }

            dbHandler.UpsertPageKeyword(artId, keyId, kw.GetMentions());

        }

        pageCount++;
    }
    
}