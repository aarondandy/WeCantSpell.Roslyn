using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Roslyn.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class ForStyleLoopSpellingTests : CSharpParsingTestBase
    {
        public static object[][] CanFindMistakesInForStyleLoopsData =>
            new[]
            {
                new object[] { "thing", 15, 26 },
                new object[] { "index", 20, 22 },
                new object[] { "jndex", 20, 33 },
                new object[] { "floating", 25, 25 }
            };

        [Theory, MemberData(nameof(CanFindMistakesInForStyleLoopsData))]
        public async Task can_find_mistakes_in_for_style_loops(
            string expectedWord,
            int expectedLine,
            int expectedCharacter
        )
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("Loops.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics
                .Should()
                .ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLineLocation(
                    expectedLine,
                    expectedCharacter,
                    expectedWord.Length,
                    "Loops.SimpleExamples.csx"
                )
                .And.HaveMessageContaining(expectedWord);
        }
    }
}
