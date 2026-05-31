class Find
{

    DBHandler dbHandler = new DBHandler();
    public List<TitleUrl> Search(string word)
    {
        var scores = new Dictionary<int, int>();

        var tokens = word.ToLower()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var r in results)
        {
            if (!scores.ContainsKey(r.ArticleId))
                scores[r.ArticleId] = 0;

            scores[r.ArticleId] += r.Count;
        }

        return scores
            .OrderByDescending(x => x.Value)
            .Select(x =>
            {
                string name = dbHandler.GetArticleName(x.Key);
                string url = dbHandler.GetUrl(x.Key);
                return new TitleUrl(name, url);
            })
            .DistinctBy(x => x.Url)
            .ToList();
    }

    public List<TitleUrl> SearchPhrase(string word)
    {
        List<TitleUrl> tuList = new List<TitleUrl>();
        int kwId = dbHandler.GetKeywordId(word);

        if (kwId == -1)
            return tuList;

        List<ArticleKeywordCount> akcList = dbHandler.GetArticleKeywordCount(kwId, 2);

        foreach (ArticleKeywordCount akc in akcList)
        {
            string name = dbHandler.GetArticleName(akc.ArticleId);
            string url = dbHandler.GetUrl(akc.ArticleId);

            tuList.Add(new TitleUrl(name, url));
            Console.WriteLine($"{name}, {url}");
        }

        return tuList;
    }

    private void MergeScores(Dictionary<int, int> scores, string word)
    {
        int kwId = dbHandler.GetKeywordId(word);

        if (kwId == -1)
            return;

        var results = dbHandler.GetArticleKeywordCount(kwId, 1);

        foreach (var r in results)
        {
            if (!scores.ContainsKey(r.ArticleId))
                scores[r.ArticleId] = 0;

            scores[r.ArticleId] += r.Count;
        }
        
        // phrase boost
        string title = dbHandler.GetArticleName(r.ArticleId);
        foreach (var r in results.Select(x => x.ArticleId).Distinct())
        {
            string title = dbHandler.GetArticleName(r);

            if (title.ToLower().Contains(word))
                scores[r] += 5;
        }
    }

}