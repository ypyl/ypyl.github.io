using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Markdig;
using Markdown.ColorCode;
using Microsoft.CodeAnalysis;

namespace SourceGenerator
{
    [Generator]
    public class ArticleeGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var markdownPipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseColorCode()
                .Build();
            var code = new StringBuilder();
            code.AppendLine(
"""""""""
using System.Collections.Generic;
namespace Blog;
public static class Articles
{
    public static IReadOnlyDictionary<string, string> Value()
    {
        return new Dictionary<string, string>
        {
""""""""");
            foreach (AdditionalText additionalFile in context.AdditionalFiles)
            {
                // Process the additional file
                var filePath = additionalFile.Path;
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var fileContent = additionalFile.GetText(context.CancellationToken);
                var parsedContext = MetaDataAndMarkdown(fileContent.ToString());
                var html = Markdig.Markdown.ToHtml(parsedContext.Item2, markdownPipeline);
                var content =
$$"""""""""
            ["""{{fileName}}"""] =
"""""
{{html}}
""""",
""""""""";
                code.AppendLine(content);
            }
            code.AppendLine(
"""""""""
        };
    }
}
""""""""");
            context.AddSource($"ArtcilesContent.g.cs", code.ToString());
        }

        private (Dictionary<string, string>, string) MetaDataAndMarkdown(string context)
        {
            var keys = new [] {"title", "date", "categories", "tags"};
            var lines = context.Split('\n');
            if (lines.Length == 0)
            {
                return (new Dictionary<string, string>(), context);
            }
            if (lines[0].Trim()!= "---")
            {
                return (new Dictionary<string, string>(), context);
            }
            var i = 1;
            var meta = new Dictionary<string, string>();
            while (i < lines.Length && lines[i].Trim() != "---")
            {
                var line = lines[i].Trim();
                var splitted = line.Split(':');
                var key = splitted[0].Trim();
                if (!keys.Any(x => x == key))
                {
                    i++;
                    continue;
                }
                var value = string.Join(':', splitted.Skip(1));
                meta.Add(key, value);
                i++;
            }
            return (meta, string.Join('\n', lines.Skip(i + 1).Select(x =>
            {
                return x.Trim();
            })));
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }
    }
}
