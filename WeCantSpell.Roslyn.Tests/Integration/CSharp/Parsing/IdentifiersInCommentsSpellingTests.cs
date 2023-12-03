using System.Threading.Tasks;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class IdentifiersInCommentsSpellingTests : CSharpParsingTestBase
    {
        [Fact]
        public async Task CanSpellIdenitifiersInComments()
        {
            var analyzer = new SpellingAnalyzerCSharp(new EmbeddedSpellChecker(new[] { "en-US", "ru-RU" }));

            var project = await ReadCodeFileAsProjectAsync("IdenitifiersInComments.Example.csx");
            project.Should().NotBeNull();

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);
            diagnostics.Should().BeEmpty();
        }
    }
}
