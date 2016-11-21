using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Tests.Integration.CSharp
{
    public class VariableDeclaratorSpellingTests : CSharpTestBase
    {
        public static IEnumerable<object[]> can_find_spelling_mistakes_in_locals_data
        {
            get
            {
                yield return new object[] { "word", 163 };
                yield return new object[] { "One", 167 };
                yield return new object[] { "phrase", 172 };
                yield return new object[] { "Two", 178 };
                yield return new object[] { "simple", 200 };
                yield return new object[] { "Name", 206 };
                yield return new object[] { "OGRE", 237 };
                yield return new object[] { "CAPS", 242 };
                yield return new object[] { "What", 292 };
                yield return new object[] { "even", 297 };
                yield return new object[] { "Is", 302 };
                yield return new object[] { "This", 304 };
                yield return new object[] { "anonymous", 328 };
            }
        }

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
    }
}
