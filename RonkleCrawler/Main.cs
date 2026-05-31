DBHandler dbHandler = new DBHandler();

dbHandler.QueueArticle("https://en.wikipedia.org/wiki/Rick_Worthy");

// limit amount of pages to crawl per run
await Crawl(500);

async Task Crawl(int maxPages)
{
    Dictionary<string, int> keywordCache = new();
    int pageCount = 0;

    while (pageCount < maxPages)
    {
        var batch = dbHandler.GetUncrawled(50);

        if (batch.Count == 0)
            break;

        foreach (var url in batch)
        {
            if (pageCount >= maxPages)
                return;

            var scraper = new Scraper(new Uri(url));
            await scraper.Scrape();

            // add new links to database, mark as uncrawled
            foreach (var link in scraper.GetLinks())
            {
                dbHandler.QueueArticle(link);
            }

            // set current article to crawled
            dbHandler.UpdateArticle(url, scraper.GetArticleName());

            AddKeywords(scraper, dbHandler.GetArticleId(url), keywordCache);

            pageCount++;
        }
    }
}

// add keywords of crawled website to db
void AddKeywords(Scraper scraper, int artId, Dictionary<string, int> keywordCache)
{
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

    dbHandler.InsertKeyword(scraper.GetArticleName());
    int kId = dbHandler.GetKeywordId(scraper.GetArticleName());

    dbHandler.UpsertPageKeyword(artId, kId, 3);
}