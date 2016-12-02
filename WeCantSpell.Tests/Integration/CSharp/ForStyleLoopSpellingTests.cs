using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Tests.Integration.CSharp
{
    public class ForStyleLoopSpellingTests : CSharpTestBase
    {
        public static IEnumerable<object[]> can_find_mistakes_in_for_style_loops_data
        {
            get
            {
                yield return new object[] { "thing", 317 };
                yield return new object[] { "index", 427 };
                yield return new object[] { "jndex", 438 };
                yield return new object[] { "floating", 564 };
            }
        }

        [Theory, MemberData(nameof(can_find_mistakes_in_for_style_loops_data))]
        public async Task can_find_mistakes_in_for_style_loops(string expectedWord, int expectedStart)
        {
            var expectedEnd = expectedStart + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("Loops.SimpleExamples.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(expectedStart, expectedEnd, "Loops.SimpleExamples.cs")
                .And.HaveMessageContaining(expectedWord);
        }
    }
}
