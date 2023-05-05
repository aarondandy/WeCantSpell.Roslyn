using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Roslyn.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class LocalDeclarationSpellingTests : CSharpParsingTestBase
    {
        public static object[][] CanFindSpellingMistakesInLocalsData =>
            new[]
            {
                new object[] { "word", 7, 17 },
                new object[] { "One", 7, 21 },
                new object[] { "phrase", 7, 26 },
                new object[] { "Two", 7, 32 },
                new object[] { "simple", 8, 17 },
                new object[] { "Name", 8, 23 },
                new object[] { "OGRE", 9, 17 },
                new object[] { "CAPS", 9, 22 },
                new object[] { "What", 10, 24 },
                new object[] { "even", 10, 29 },
                new object[] { "Is", 10, 34 },
                new object[] { "This", 10, 36 },
                new object[] { "anonymous", 11, 17 },
                new object[] { "readonly", 12, 27 },
                new object[] { "what", 13, 21 }
            };

        [Theory, MemberData(nameof(CanFindSpellingMistakesInLocalsData))]
        public async Task can_find_spelling_mistakes_in_locals(
            string expectedWord,
            int expectedLine,
            int expectedCharacter
        )
        {
            var expectedEnd = expectedLine + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("LocalVariables.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics
                .Should()
                .ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLineLocation(
                    expectedLine,
                    expectedCharacter,
                    expectedWord.Length,
                    "LocalVariables.SimpleExamples.csx"
                )
                .And.HaveMessageContaining(expectedWord);
        }

        [Fact]
        public async Task no_diagnostics_when_all_words_are_ok()
        {
            var analyzer = new SpellingAnalyzerCSharp(new AllGoodWordChecker());
            var project = await ReadCodeFileAsProjectAsync("LocalVariables.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().BeEmpty();
        }

        [Fact]
        public async Task at_symbol_is_not_checked()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("@", "@readonly"));
            var project = await ReadCodeFileAsProjectAsync("LocalVariables.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().BeEmpty();
        }
    }
}
