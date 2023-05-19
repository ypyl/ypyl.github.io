using System.IO;
using System.Text;
using Markdig;
using Markdown.ColorCode;
using Microsoft.CodeAnalysis;

namespace SourceGenerator
{
    [Generator]
    public class MineSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var MarkdownPipeline = new MarkdownPipelineBuilder()
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
    public static IReadOnlyDictionary<string, string> Content()
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
                var html = Markdig.Markdown.ToHtml(fileContent.ToString(), MarkdownPipeline);
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

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }
    }
}
