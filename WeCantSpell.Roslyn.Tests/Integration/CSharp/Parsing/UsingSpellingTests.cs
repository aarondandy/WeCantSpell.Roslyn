using System.Threading.Tasks;
using WeCantSpell.Roslyn.Tests.Utilities;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class UsingSpellingTests : CSharpParsingTestBase
    {
        public static object[][] CanFindSpellingMistakesInUsingsData =>
            new[]
            {
                new object[] { "Nope", 5, 7 },
                new object[] { "Not", 6, 7 },
                new object[] { "Done", 6, 10 },
                new object[] { "bytes", 19, 24 }
            };

        [Theory, MemberData(nameof(CanFindSpellingMistakesInUsingsData))]
        public async Task can_find_spelling_mistakes_in_usings(
            string expectedWord,
            int expectedLine,
            int expectedCharacter
        )
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("Using.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics
                .Should()
                .ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLineLocation(expectedLine, expectedCharacter, expectedWord.Length, "Using.SimpleExamples.csx")
                .And.HaveMessageContaining(expectedWord);
        }
    }
}
