using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Tests.Integration.CSharp
{
    public class UsingSpellingTests : CSharpTestBase
    {
        public static IEnumerable<object[]> can_find_spelling_mistakes_in_usings_data
        {
            get
            {
                yield return new object[] { "Nope", 52 };
                yield return new object[] { "Not", 96 };
                yield return new object[] { "Done", 99 };
            }
        }

        [Theory, MemberData(nameof(can_find_spelling_mistakes_in_usings_data))]
        public async Task can_find_spelling_mistakes_in_usings(string expectedWord, int expectedStart)
        {
            var expectedEnd = expectedStart + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("Using.SimpleExamples.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(expectedStart, expectedEnd, "Using.SimpleExamples.cs")
                .And.HaveMessageContaining(expectedWord);
        }
    }
}
