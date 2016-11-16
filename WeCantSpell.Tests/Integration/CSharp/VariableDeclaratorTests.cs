using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Tests.Integration.CSharp
{
    public class VariableDeclaratorTests : CSharpTestBase
    {
        public static IEnumerable<object[]> can_find_spelling_mistakes_in_locals_data
        {
            get
            {
                yield return new object[] { "word", 159 };
                yield return new object[] { "One", 163 };
                yield return new object[] { "phrase", 168 };
                yield return new object[] { "Two", 174 };
                yield return new object[] { "simple", 196 };
                yield return new object[] { "Name", 202 };
                yield return new object[] { "OGRE", 233 };
                yield return new object[] { "CAPS", 238 };
                yield return new object[] { "What", 288 };
                yield return new object[] { "even", 293 };
                yield return new object[] { "Is", 298 };
                yield return new object[] { "This", 300 };
                yield return new object[] { "anonymous", 324 };
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
