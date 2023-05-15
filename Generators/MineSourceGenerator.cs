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
            var html = "<span>My name is</span>"; //Markdig.Markdown.ToHtml(File.ReadAllText("../Blog/2010-10-16-Linq-and-Group.md"), MarkdownPipeline);
            context.AddSource("TestGeneratorName.g.cs", $$"""
namespace TestGenerator;
public static class TestGeneratorNameMine
{
    public static string BlogHtml() => "{{html}}";
}
""");
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }
    }
}
