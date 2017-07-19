using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Roslyn.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class EnumDeclarationSpellingTests : CSharpParsingTestBase
    {
        public static object[][] can_find_mistakes_in_enums_data => new[]
        {
            new object[] { "Example", 73 },
            new object[] { "Value", 101 },
            new object[] { "One", 106 },
            new object[] { "Number", 124 },
            new object[] { "Two", 130 }
        };

        [Theory, MemberData(nameof(can_find_mistakes_in_enums_data))]
        public async Task can_find_mistakes_in_enums(string expectedWord, int expectedStart)
        {
            var expectedEnd = expectedStart + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("Enum.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(expectedStart, expectedEnd, "Enum.SimpleExamples.csx")
                .And.HaveMessageContaining(expectedWord);
        }
    }
}
