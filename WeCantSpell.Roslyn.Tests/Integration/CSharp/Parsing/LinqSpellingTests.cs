using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Roslyn.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class LinqSpellingTests : CSharpParsingTestBase
    {
        public static object[][] CanFindSpellingMistakesInLinqData =>
            new[]
            {
                new object[] { "number", 11, 22 },
                new object[] { "odd", 12, 22 },
                new object[] { "intermediary", 14, 21 },
                new object[] { "Max", 16, 21 },
                new object[] { "Value", 17, 21 },
                new object[] { "group", 19, 64 },
                new object[] { "Odd", 23, 21 },
                new object[] { "Values", 24, 21 },
                new object[] { "letterint", 28, 32 },
                new object[] { "potato", 31, 32 },
                new object[] { "goat", 32, 32 },
                new object[] { "apple", 33, 32 },
                new object[] { "basket", 33, 86 },
                new object[] { "Category", 36, 31 },
                new object[] { "Produce", 37, 31 }
            };

        [Theory, MemberData(nameof(CanFindSpellingMistakesInLinqData))]
        public async Task can_find_spelling_mistakes_in_linq(
            string expectedWord,
            int expectedStart,
            int expectedCharacter
        )
        {
            var expectedEnd = expectedStart + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("Linq.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics
                .Should()
                .ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLineLocation(expectedStart, expectedCharacter, expectedWord.Length, "Linq.SimpleExamples.csx")
                .And.HaveMessageContaining(expectedWord);
        }
    }
}
