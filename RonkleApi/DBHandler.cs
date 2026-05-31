using Microsoft.Data.Sqlite;

// Open or create a database file
/*using var connection = new SqliteConnection("Data Source=wikipedia.db");
connection.Open();*/

class DBHandler
{
    private readonly SqliteConnection _db;

    public DBHandler()
    {
        _db = new SqliteConnection("Data Source=../RonkleCrawler/wikipedia.db");
        _db.Open();
    }

    public int GetCount(int articleId, int keywordId)
    {
        using var cmd = _db.CreateCommand();
        cmd.CommandText = @"SELECT count FROM Page_Keyword WHERE ArticleId = @aid AND KeywordId = @kid";
        cmd.Parameters.AddWithValue("@aid", articleId);
        cmd.Parameters.AddWithValue("@kid", keywordId);

        var result = cmd.ExecuteScalar();

        if (result == null || result == DBNull.Value)
            throw new Exception($"Count not found for: ArticleId: {articleId} and KeywordId: {keywordId}");

        return Convert.ToInt32(result);
    }

    public bool PageKeywordExists(int articleId, int keywordId)
    {
        using var cmd = _db.CreateCommand();
        cmd.CommandText = @"SELECT Count FROM Page_Keyword WHERE ArticleId = @aid AND KeywordId = @kid";
        cmd.Parameters.AddWithValue("@aid", articleId);
        cmd.Parameters.AddWithValue("@kid", keywordId);

        var result = cmd.ExecuteScalar();

        if (result == null || result == DBNull.Value)
            return false;

        return true;
    }

    public List<KeyWord> GetKeyWords(int articleId)
    {
        using var cmd = _db.CreateCommand();

        cmd.CommandText = @"
            SELECT k.Keyword, pk.Count
            FROM Page_Keyword pk
            JOIN Keyword k ON pk.KeywordId = k.Id
            WHERE pk.ArticleId = @aid;
        ";

        cmd.Parameters.AddWithValue("@aid", articleId);

        using var reader = cmd.ExecuteReader();

        List<KeyWord> keywords = new List<KeyWord>();

        while (reader.Read())
        {
            string word = reader.GetString(0);
            int count = reader.GetInt32(1);

            keywords.Add(new KeyWord(word, count));
        }

        return keywords;
    }

    // lets the user get articleId based on url
    public int GetArticleId(string url)
    {
        using var cmd = _db.CreateCommand();
        cmd.CommandText = @"SELECT Id FROM Article WHERE Url = @url";
        cmd.Parameters.AddWithValue("@url", url);

        var result = cmd.ExecuteScalar();

        if (result == null || result == DBNull.Value)
            throw new Exception($"Article not found for URL: {url}");

        return Convert.ToInt32(result);
    }

    public string GetArticleName(int id)
    {
        using var cmd = _db.CreateCommand();
        cmd.CommandText = @"SELECT Title FROM Article WHERE Id = @id";
        cmd.Parameters.AddWithValue("@id", id);

        var result = cmd.ExecuteScalar();

        if (result == null || result == DBNull.Value)
            throw new Exception($"Article not found for Id: {id}");

        return Convert.ToString(result);
    }

    // lets the user get keywordId based on word
    public int GetKeywordId(string word)
    {
        using var cmd = _db.CreateCommand();
        cmd.CommandText = @"SELECT Id FROM Keyword WHERE Keyword LIKE @word OR @word LIKE Keyword";
        cmd.Parameters.AddWithValue("@word", "%" + word + "%");

        var result = cmd.ExecuteScalar();

        if (result == null || result == DBNull.Value)
            return -1;

        return Convert.ToInt32(result);
    }

    public bool KeywordExists(string keyWord)
    {
        using var cmd = _db.CreateCommand();
        cmd.CommandText = @"SELECT Id FROM Keyword WHERE Keyword LIKE @word OR @word LIKE Keyword";
        cmd.Parameters.AddWithValue("@word", "%" + keyWord + "%");

        var result = cmd.ExecuteScalar();

        if (result == null || result == DBNull.Value)
            return false;

        return true;
    }

    public string GetUrl(int articleId)
    {
        using var cmd = _db.CreateCommand();
        cmd.CommandText = @"SELECT Url FROM Article WHERE Id = @aId;";
        cmd.Parameters.AddWithValue("@aId", articleId);
        var res = cmd.ExecuteScalar();

        return Convert.ToString(res);
    }

    public List<ArticleKeywordCount> GetArticleKeywordCount(int keywordId, int minCount)
    {
        List<ArticleKeywordCount> akcList = new List<ArticleKeywordCount>();
        using var cmd = _db.CreateCommand();
        cmd.CommandText = @"SELECT ArticleId, Count FROM Page_Keyword WHERE KeywordId = @kwId ORDER BY Count DESC;";
        cmd.Parameters.AddWithValue("@kwId", keywordId);


        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            int articleId = reader.GetInt32(0);
            int count = reader.GetInt32(1);

            if (count >= minCount)
                akcList.Add(new ArticleKeywordCount(articleId, count));
        }

        return akcList;
    }
}