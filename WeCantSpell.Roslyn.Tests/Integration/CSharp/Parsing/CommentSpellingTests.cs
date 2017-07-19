using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Roslyn.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class CommentSpellingTests : CSharpParsingTestBase
    {
        public static object[][] can_find_mistakes_in_comments_data => new[]
        {
            new object[] { "aardvark", 660, "SP3112" },
            new object[] { "simple", 1186, "SP3112" },
            new object[] { "under", 1235, "SP3112" },
            new object[] { "within", 127, "SP3113" },
            new object[] { "tag", 320, "SP3113" },
            new object[] { "Here", 898, "SP3113" },
            new object[] { "Just", 1130, "SP3113" }
        };

        [Theory, MemberData(nameof(can_find_mistakes_in_comments_data))]
        public async Task can_find_mistakes_in_comments(string expectedWord, int expectedStart, string expectedDiagnosticId)
        {
            var expectedEnd = expectedStart + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("XmlDoc.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId(expectedDiagnosticId)
                .And.HaveLocation(expectedStart, expectedEnd, "XmlDoc.SimpleExamples.csx")
                .And.HaveMessageContaining(expectedWord);
        }

        [Fact]
        public async Task words_in_c_elements_are_ignored()
        {
            var word = "inline";
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(word));
            var project = await ReadCodeFileAsProjectAsync("XmlDoc.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().BeEmpty();
        }

        [Fact]
        public async Task words_in_code_elements_are_ignored()
        {
            var word = "ignored";
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(word));
            var project = await ReadCodeFileAsProjectAsync("XmlDoc.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().BeEmpty();
        }
    }
}
