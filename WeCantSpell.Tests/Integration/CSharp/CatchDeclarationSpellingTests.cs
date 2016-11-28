using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Tests.Integration.CSharp
{
    public class CatchDeclarationSpellingTests : CSharpTestBase
    {
        [Fact]
        public async Task can_check_spelling_of_catchs()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("bad", "Value"));
            var project = await ReadCodeFileAsProjectAsync("Catch.SimpleExamples.cs");

            var diagnostics = (await GetDiagnosticsAsync(project, analyzer)).ToList();

            diagnostics.Should().HaveCount(2);
            diagnostics[0].Should().HaveMessageContaining("bad")
                .And.HaveLocation(309, 312, "Catch.SimpleExamples.cs");
            diagnostics[1].Should().HaveMessageContaining("Value")
                .And.HaveLocation(312, 317, "Catch.SimpleExamples.cs");
        }
    }
}
