class KeyWord
{
    private readonly string _word;
    private int _mentions;
    public KeyWord(string word, int mentions=0)
    {
        _word = word;
        _mentions = mentions;
    }

    public int GetMentions()
    {
        return _mentions;
    }

    public void SetMentions(int mentions)
    {
        _mentions = mentions;
    }

    public string GetWord()
    {
        return _word;
    }
}