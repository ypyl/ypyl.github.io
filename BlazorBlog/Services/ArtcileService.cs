namespace Services;

class ArticleService
{
    public IEnumerable<string> ArticleNames()
    {
        var result = new List<(int, string)>();
        foreach (var articleName in Blog.Articles.Value().Keys)
        {
            var parser = articleName.Split("-");
            var year = Convert.ToInt32(parser[0]);
            var month = Convert.ToInt32(parser[1]);
            var day = Convert.ToInt32(parser[2]);
            result.Add((year * 10000 + month * 100 + day, articleName));
        }
        return result.OrderByDescending(x => x.Item1).Select(x => x.Item2);
    }

    public string Content(string articleName)
    {
        return Blog.Articles.Value()[articleName];
    }
}
