using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Markdig;
using Markdown.ColorCode;
using Microsoft.CodeAnalysis;
using System;

namespace SourceGenerator;

[Generator]
public class ArticleGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext initContext)
    {
        var markdownPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseMathematics()
            .UseColorCode()
            .Build();

        IncrementalValuesProvider<AdditionalText> additionalTexts = initContext.AdditionalTextsProvider.Where(static file => file.Path.EndsWith(".md"));

        var collected = additionalTexts.Collect();

        var transformed = collected.Select((mdFiles, token) =>
        {
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
            foreach (AdditionalText additionalFile in mdFiles)
            {
                // Process the additional file
                var filePath = additionalFile.Path;
                var fileName = Path.GetFileNameWithoutExtension(filePath).Replace(".", string.Empty);
                var fileContent = additionalFile.GetText(token);
                var parsedContext = MetaDataAndMarkdown(fileContent.ToString());
                if (!parsedContext.Item1.Any())
                {
                    continue;
                }
                var html = Markdig.Markdown.ToHtml(parsedContext.Item2, markdownPipeline);
                var meta = new StringBuilder();
                var categories = string.Join("|", Categories(filePath));
                meta.Append($"[\"path\"]=\"{categories}\",");
                foreach (var keyValue in parsedContext.Item1)
                {
                    var key = keyValue.Key;
                    var value = keyValue.Value;
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
                if (parsedContext.Item1.All(x => x.Key != "date"))
                {
                    var singleMeta =
                        $$"""""""""
                            ["""date"""] =
                            """""
                            {{ArticleCreatedDate(fileName)}}
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
            return code.ToString();
        });

        // generate a class that contains their values as const strings
        initContext.RegisterSourceOutput(transformed, (spc, finalFile) =>
        {
            spc.AddSource("ArtcilesContent.g.cs", finalFile);
        });
    }

    public string ArticleCreatedDate(string articleName)
    {
        var parser = articleName.Split('-');
        var year = Convert.ToInt32(parser[0]);
        var month = Convert.ToInt32(parser[1]);
        var day = Convert.ToInt32(parser[2]);

        return $"{year}-{month}-{day}";
    }

    private (Dictionary<string, string>, string) MetaDataAndMarkdown(string context)
    {
        var keys = new[] { "title", "date", "categories", "tags" };
        var lines = context.Split('\n');
        if (lines.Length == 0)
        {
            return (new Dictionary<string, string>(), context);
        }
        if (lines[0].Trim() != "---")
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
            var value = string.Join(":", splitted.Skip(1));
            meta.Add(key, value);
            i++;
        }
        return (meta, string.Join("\n", lines.Skip(i + 1).Select(x =>
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
