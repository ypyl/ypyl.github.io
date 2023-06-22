namespace Services;

public class Folder
{
    public string? Name { get; set; }
    public List<Folder> Folders { get; set; } = new List<Folder>();
    public List<(DateTime, string)> Files { get; set; } = new List<(DateTime, string)>();
}

class ArticleService
{
    public Folder Articles()
    {
        var resultFolder = new Folder();
        foreach (var articleName in Blog.Articles.Value().Keys)
        {
            var parser = articleName.Split("-");
            var year = Convert.ToInt32(parser[0]);
            var month = Convert.ToInt32(parser[1]);
            var day = Convert.ToInt32(parser[2]);

            var createdDate = new DateTime(year, month, day);

            var categories = Blog.Articles.Value()[articleName].Item1["path"].Split("|", StringSplitOptions.RemoveEmptyEntries);
            var targetFolder = GetTargetFolder(resultFolder, categories);
            targetFolder.Files.Add((createdDate, articleName));
        }
        return resultFolder;
    }

    public IEnumerable<string> ArticleNames(Folder? folder, string? searchTerm)
    {
        if (folder is null)
        {
            return Enumerable.Empty<string>();
        }
        return folder.Files.OrderByDescending(x => x.Item1).Select(x => x.Item2).Where(x => x.Contains(searchTerm ?? string.Empty, StringComparison.OrdinalIgnoreCase));
    }

    private Folder GetTargetFolder(Folder root, string[] categories)
    {
        var target = root;
        foreach (var category in categories)
        {
            if (target.Folders.Any(x => x.Name == category))
            {
                target = target.Folders.First(x => x.Name == category);
            }
            else
            {
                var newFolder = new Folder() { Name = category };
                target.Folders.Add(newFolder);
                target = newFolder;
            }
        }
        return target;
    }

    public string Content(string articleName)
    {
        return Blog.Articles.Value()[articleName].Item2;
    }

    public DateTime ArticleCreatedDate(string articleName)
    {
        var parser = articleName.Split("-");
        var year = Convert.ToInt32(parser[0]);
        var month = Convert.ToInt32(parser[1]);
        var day = Convert.ToInt32(parser[2]);

        return new DateTime(year, month, day);
    }

    public string Title(string? articleName)
    {
        if (articleName is null)
        {
            return string.Empty;
        }
        if (Blog.Articles.Value()[articleName].Item1.TryGetValue("title", out var title))
        {
            return title.Trim().Trim('\"');
        }
        return string.Empty;
    }

    public string Categories(string? articleName)
    {
        if (articleName is null)
        {
            return string.Empty;
        }
        if (Blog.Articles.Value()[articleName].Item1.TryGetValue("categories", out var categories))
        {
            return categories;
        }
        return string.Empty;
    }

    public IEnumerable<string> Tags(string? articleName)
    {
        if (articleName is null)
        {
            return Enumerable.Empty<string>();
        }
        if (Blog.Articles.Value()[articleName].Item1.TryGetValue("tags", out var categories))
        {
            return categories.Split(",", StringSplitOptions.RemoveEmptyEntries);
        }
        return Enumerable.Empty<string>();
    }

    public string Date(string? articleName)
    {
        if (articleName is null)
        {
            return string.Empty;
        }
        if (Blog.Articles.Value()[articleName].Item1.TryGetValue("date", out var categories))
        {
            return categories;
        }
        return string.Empty;
    }

    public bool IsThereArticleWithName(string? name)
    {
        return name is not null && Blog.Articles.Value().ContainsKey(name);
    }

    public string FirstArticle()
    {
        return Blog.Articles.Value().First().Key;
    }
}
