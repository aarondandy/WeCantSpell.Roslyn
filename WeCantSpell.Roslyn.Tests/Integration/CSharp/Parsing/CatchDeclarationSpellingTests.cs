using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Roslyn.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class CatchDeclarationSpellingTests : CSharpParsingTestBase
    {
        [Fact]
        public async Task can_check_spelling_of_catchs()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("bad", "Value"));
            var project = await ReadCodeFileAsProjectAsync("Catch.SimpleExamples.csx");

            var diagnostics = (await GetDiagnosticsAsync(project, analyzer)).ToList();

            diagnostics.Should().HaveCount(2);
            diagnostics[0].Should().HaveMessageContaining("bad")
                .And.HaveLocation(309, 312, "Catch.SimpleExamples.csx");
            diagnostics[1].Should().HaveMessageContaining("Value")
                .And.HaveLocation(312, 317, "Catch.SimpleExamples.csx");
        }
    }
}
