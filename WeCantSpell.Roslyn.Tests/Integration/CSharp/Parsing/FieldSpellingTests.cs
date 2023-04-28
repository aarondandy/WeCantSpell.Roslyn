using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Roslyn.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class FieldSpellingTests : CSharpParsingTestBase
    {
        public static object[][] CanFindMistakesInVariousFieldsData => new[]
        {
            new object[] { "read", 5, 32 },
            new object[] { "Only", 5, 36 },
            new object[] { "hidden", 7, 25 },
            new object[] { "Value", 9, 20 },
            new object[] { "Count", 9, 32 },
            new object[] { "const", 11, 30 },
            new object[] { "what", 13, 24 }
        };

        [Theory, MemberData(nameof(CanFindMistakesInVariousFieldsData))]
        public async Task can_find_mistakes_in_various_fields(string expectedWord, int expectedLine,
            int expectedCharacter
        )
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("Fields.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLineLocation(expectedLine, expectedCharacter, expectedWord.Length, "Fields.SimpleExamples.csx")
                .And.HaveMessageContaining(expectedWord);
        }

        [Fact]
        public async Task common_prefixes_are_ignored()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("_", "m", "m_"));
            var project = await ReadCodeFileAsProjectAsync("Fields.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().BeEmpty();
        }
    }
}
