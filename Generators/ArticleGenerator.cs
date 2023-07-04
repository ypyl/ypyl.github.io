﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Markdig;
using Markdown.ColorCode;
using Microsoft.CodeAnalysis;

namespace SourceGenerator
{
    [Generator]
    public class ArticleGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var markdownPipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseMathematics()
                .UseColorCode()
                .Build();
            var code = new StringBuilder();
            code.AppendLine(
"""""""""
using System.Collections.Generic;
namespace Blog;
public static class Articles
{
    public static Dictionary<string, (Dictionary<string, string>, string)> Value()
    {
        return new Dictionary<string, (Dictionary<string, string>, string)>
        {
""""""""");
            foreach (AdditionalText additionalFile in context.AdditionalFiles)
            {
                // Process the additional file
                var filePath = additionalFile.Path;
                var fileName = Path.GetFileNameWithoutExtension(filePath).Replace(".", string.Empty);
                var fileContent = additionalFile.GetText(context.CancellationToken);
                var parsedContext = MetaDataAndMarkdown(fileContent.ToString());
                if (!parsedContext.Item1.Any())
                {
                    continue;
                }
                var html = Markdig.Markdown.ToHtml(parsedContext.Item2, markdownPipeline);
                var meta = new StringBuilder();
                var categories = string.Join("|", Categories(filePath));
                meta.Append($"[\"path\"]=\"{categories}\",");
                foreach (var (key, value) in parsedContext.Item1)
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        continue;
                    }
                    var singleMeta =
$$"""""""""
["""{{key}}"""] =
"""""
{{value}}
""""",
""""""""";
                    meta.AppendLine(singleMeta);
                }
                var content =
$$"""""""""
            ["""{{fileName}}"""] = (new Dictionary<string, string> { {{meta}} },
"""""
{{html}}
"""""
),
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
                return x;
            })));
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }

        private string[] Categories(string filepath)
        {
            const string block = "Blog";
            var splitted = filepath.Split(Path.DirectorySeparatorChar);
            var result = new List<string>();
            var i = 0;
            while (i < 6)
            {
                if (splitted.Length - 2 - i <= 0)
                {
                    break;
                }
                var current = splitted[splitted.Length - 2 - i];
                if (current == block)
                {
                    break;
                }
                result.Add(current);
                i++;
            }
            result.Reverse();
            return result.ToArray();
        }
    }
}
