namespace Services;

public class Folder
{
    public string? Name { get; set; }
    public List<Folder> Folders { get; set; } = new List<Folder>();
    public List<(int, string)> Files { get; set; } = new List<(int, string)>();
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

            var categories = Blog.Articles.Value()[articleName].Item1["path"].Split("|", StringSplitOptions.RemoveEmptyEntries);
            var targetFolder = GetTargetFolder(resultFolder, categories);
            targetFolder.Files.Add((year * 10000 + month * 100 + day, articleName));
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

    public bool IsThereArticleWithName(string? name)
    {
        return name is not null && Blog.Articles.Value().ContainsKey(name);
    }

    public bool IsThereAnyArticle()
    {
        return Blog.Articles.Value().Any();
    }

    public string AnyContent()
    {
        return Blog.Articles.Value().First().Value.Item2;
    }
}
