using System.Threading.Tasks;
using WeCantSpell.Roslyn.Tests.Utilities;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class CommentSpellingTests : CSharpParsingTestBase
    {
        public static object[][] CanFindMistakesInCommentsData =>
            new[]
            {
                new object[] { "aardvark", 25, 16, "SP3112" },
                new object[] { "simple", 47, 18, "SP3112" },
                new object[] { "under", 48, 34, "SP3112" },
                new object[] { "within", 4, 52, "SP3113" },
                new object[] { "tag", 10, 18, "SP3113" },
                new object[] { "Here", 37, 12, "SP3113" },
                // TODO: WriteLine in comments should be considered as CamelCase identifier and parsed accordingly
                // new object[] { "Line", 17, 26, "SP3113" },
                new object[] { "Just", 45, 17, "SP3113" }
            };

        [Theory, MemberData(nameof(CanFindMistakesInCommentsData))]
        public async Task can_find_mistakes_in_comments(
            string expectedWord,
            int expectedLine,
            int expectedCharacter,
            string expectedDiagnosticId
        )
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("XmlDoc.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics
                .Should()
                .ContainSingle()
                .Subject.Should()
                .HaveId(expectedDiagnosticId)
                .And.HaveLineLocation(expectedLine, expectedCharacter, expectedWord.Length, "XmlDoc.SimpleExamples.csx")
                .And.HaveMessageContaining(expectedWord);
        }

        [Fact]
        public async Task words_in_c_elements_are_ignored()
        {
            const string word = "inline";
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(word));
            var project = await ReadCodeFileAsProjectAsync("XmlDoc.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().BeEmpty();
        }

        [Fact]
        public async Task words_in_code_elements_are_ignored()
        {
            const string word = "ignored";
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(word));
            var project = await ReadCodeFileAsProjectAsync("XmlDoc.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().BeEmpty();
        }
    }
}
