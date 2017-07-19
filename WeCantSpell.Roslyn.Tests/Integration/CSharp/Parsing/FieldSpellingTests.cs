using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Roslyn.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class FieldSpellingTests : CSharpParsingTestBase
    {
        public static object[][] can_find_mistakes_in_various_fields_data => new[]
        {
            new object[] { "read", 124 },
            new object[] { "Only", 128 },
            new object[] { "hidden", 166 },
            new object[] { "Value", 211 },
            new object[] { "Count", 223 },
            new object[] { "const", 266 },
            new object[] { "what", 305 }
        };

        [Theory, MemberData(nameof(can_find_mistakes_in_various_fields_data))]
        public async Task can_find_mistakes_in_various_fields(string expectedWord, int expectedStart)
        {
            var expectedEnd = expectedStart + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("Fields.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(expectedStart, expectedEnd, "Fields.SimpleExamples.csx")
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
