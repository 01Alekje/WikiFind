class Find
{

    DBHandler dbHandler = new DBHandler();
    public List<TitleUrl> Search(string word)
    {
        var scores = new Dictionary<int, int>();

        var tokens = word.ToLower()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var token in tokens)
        {
            MergeScores(scores, token);
        }

        return scores
            .GroupBy(x => x.Key)
            .Select(g => new
            {
                ArticleId = g.Key,
                Score = g.Sum(x => x.Value)
            })
            .OrderByDescending(x => x.Score)
            .Select(x =>
            {
                string name = dbHandler.GetArticleName(x.ArticleId);
                string url = dbHandler.GetUrl(x.ArticleId);
                return new TitleUrl(name, url);
            })
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

        var results = dbHandler.GetArticleKeywordCount(kwId, 2);

        foreach (var r in results)
        {
            if (!scores.ContainsKey(r.ArticleId))
                scores[r.ArticleId] = 0;

            scores[r.ArticleId] += r.Count;
        }

        // phrase boost (must be OUTSIDE loop)
        foreach (var r in results.Select(x => x.ArticleId).Distinct())
        {
            string title = dbHandler.GetArticleName(r);

            if (title.ToLower().Contains(word))
                scores[r] += 5;
        }
    }

}