using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Roslyn.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp
{
    public class LocalDeclarationSpellingTests : CSharpTestBase
    {
        public static object[][] can_find_spelling_mistakes_in_locals_data => new[]
        {
            new object[] { "word", 163 },
            new object[] { "One", 167 },
            new object[] { "phrase", 172 },
            new object[] { "Two", 178 },
            new object[] { "simple", 200 },
            new object[] { "Name", 206 },
            new object[] { "OGRE", 237 },
            new object[] { "CAPS", 242 },
            new object[] { "What", 292 },
            new object[] { "even", 297 },
            new object[] { "Is", 302 },
            new object[] { "This", 304 },
            new object[] { "anonymous", 328 },
            new object[] { "readonly", 376 },
            new object[] { "what", 417 }
        };

        [Theory, MemberData(nameof(can_find_spelling_mistakes_in_locals_data))]
        public async Task can_find_spelling_mistakes_in_locals(string expectedWord, int expectedStart)
        {
            var expectedEnd = expectedStart + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("LocalVariables.SimpleExamples.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(expectedStart, expectedEnd, "LocalVariables.SimpleExamples.cs")
                .And.HaveMessageContaining(expectedWord);
        }

        [Fact]
        public async Task no_diagnostics_when_all_words_are_ok()
        {
            var analyzer = new SpellingAnalyzerCSharp(new AllGoodWordChecker());
            var project = await ReadCodeFileAsProjectAsync("LocalVariables.SimpleExamples.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().BeEmpty();
        }

        [Fact]
        public async Task at_symbol_is_not_checked()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("@", "@readonly"));
            var project = await ReadCodeFileAsProjectAsync("LocalVariables.SimpleExamples.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().BeEmpty();
        }
    }
}
