using System.Threading.Tasks;
using WeCantSpell.Roslyn.Tests.Utilities;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class EnumDeclarationSpellingTests : CSharpParsingTestBase
    {
        public static object[][] CanFindMistakesInEnumsData =>
            new[]
            {
                new object[] { "Example", 3, 17 },
                new object[] { "Value", 5, 9 },
                new object[] { "One", 5, 14 },
                new object[] { "Number", 6, 9 },
                new object[] { "Two", 6, 15 }
            };

        [Theory, MemberData(nameof(CanFindMistakesInEnumsData))]
        public async Task can_find_mistakes_in_enums(string expectedWord, int expectedLine, int expectedCharacter)
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("Enum.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics
                .Should()
                .ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLineLocation(expectedLine, expectedCharacter, expectedWord.Length, "Enum.SimpleExamples.csx")
                .And.HaveMessageContaining(expectedWord);
        }
    }
}
