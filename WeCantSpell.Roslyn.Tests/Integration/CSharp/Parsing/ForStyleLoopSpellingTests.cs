using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Roslyn.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class ForStyleLoopSpellingTests : CSharpParsingTestBase
    {
        public static object[][] can_find_mistakes_in_for_style_loops_data => new[]
        {
            new object[] { "thing", 317 },
            new object[] { "index", 427 },
            new object[] { "jndex", 438 },
            new object[] { "floating", 564 }
        };

        [Theory, MemberData(nameof(can_find_mistakes_in_for_style_loops_data))]
        public async Task can_find_mistakes_in_for_style_loops(string expectedWord, int expectedStart)
        {
            var expectedEnd = expectedStart + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("Loops.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(expectedStart, expectedEnd, "Loops.SimpleExamples.csx")
                .And.HaveMessageContaining(expectedWord);
        }
    }
}
