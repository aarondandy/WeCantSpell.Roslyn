using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Roslyn.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp
{
    public class LinqSpellingTests : CSharpTestBase
    {
        public static object[][] can_find_spelling_mistakes_in_linq_data => new[]
        {
            new object[] { "number", 223 },
            new object[] { "odd", 279 },
            new object[] { "intermediary", 371 },
            new object[] { "Max", 430 },
            new object[] { "Value", 465 },
            new object[] { "group", 560 },
            new object[] { "Odd", 681 },
            new object[] { "Values", 720 },
            new object[] { "letterint", 820 },
            new object[] { "potato", 943 },
            new object[] { "goat", 993 },
            new object[] { "apple", 1047 },
            new object[] { "basket", 1101 },
            new object[] { "Category", 1206 },
            new object[] { "Produce", 1260 }
        };

        [Theory, MemberData(nameof(can_find_spelling_mistakes_in_linq_data))]
        public async Task can_find_spelling_mistakes_in_linq(string expectedWord, int expectedStart)
        {
            var expectedEnd = expectedStart + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("Linq.SimpleExamples.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(expectedStart, expectedEnd, "Linq.SimpleExamples.cs")
                .And.HaveMessageContaining(expectedWord);
        }
    }
}
