using System.IO;
using Markdig;
using Markdown.ColorCode;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace SourceGenerator
{
    [Generator]
    public class MineSourceGenerator : ISourceGenerator
    {
        private static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseColorCode()
            .Build();

        public void Execute(GeneratorExecutionContext context)
        {
            foreach (AdditionalText additionalFile in context.AdditionalFiles)
            {
                // Process the additional file
                var filePath = additionalFile.Path;
                var fileName = Path.GetFileName(filePath);
                var fileContent = additionalFile.GetText(context.CancellationToken);
                var html = Markdig.Markdown.ToHtml(fileContent.ToString(), MarkdownPipeline);
                context.AddSource($"{fileName}.g.cs", $$"""
                    namespace TestGenerator;
                    public static class TestGeneratorNameMine
                    {
                        public static string BlogHtml() => "{{html}}";
                    }
                    """);
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }
    }
}
