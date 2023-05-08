using System.Threading.Tasks;
using WeCantSpell.Roslyn.Tests.Utilities;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class StringLiteralSpellingTests : CSharpParsingTestBase
    {
        public static object[][] CanFindMistakesInLiteralsData =>
            new[]
            {
                new object[] { "apple", 7, 32 },
                new object[] { "banana", 7, 38 },
                new object[] { "cranberry", 7, 45 },
                new object[] { "dragon-fruit", 11, 26 },
                new object[] { "edamame", 11, 40 },
                new object[] { "fig", 11, 48 },
                new object[] { "gooseberry", 12, 34 },
                new object[] { "huckleberry", 12, 47 },
                new object[] { "イチゴ", 12, 61 },
                new object[] { "jackfruit", 13, 34 },
                new object[] { "kiwi", 14, 3 },
                new object[] { "lemon", 14, 12 },
                new object[] { "mango", 15, 3 },
                new object[] { "nectarine", 15, 12 },
                new object[] { "orange", 15, 24 },
                new object[] { "papaya", 15, 32 },
                new object[] { "quince", 16, 3 },
                new object[] { "raspberry", 16, 13 },
                new object[] { "strawberry", 17, 34 },
                new object[] { "shake", 17, 47 },
                new object[] { "tomato", 18, 33 },
                new object[] { "ugli", 18, 41 }
            };

        [Theory, MemberData(nameof(CanFindMistakesInLiteralsData))]
        public async Task can_find_mistakes_in_literals(string expectedWord, int expectedLine, int expectedCharacter)
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("StringLiteral.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics
                .Should()
                .ContainSingle()
                .Subject.Should()
                .HaveId("SP3111")
                .And.HaveLineLocation(
                    expectedLine,
                    expectedCharacter,
                    expectedWord.Length,
                    "StringLiteral.SimpleExamples.csx"
                )
                .And.HaveMessageContaining(expectedWord);
        }
    }
}
