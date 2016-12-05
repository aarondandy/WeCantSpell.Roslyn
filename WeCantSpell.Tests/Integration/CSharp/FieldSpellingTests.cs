using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Tests.Integration.CSharp
{
    public class FieldSpellingTests : CSharpTestBase
    {
        public static IEnumerable<object[]> can_find_mistakes_in_various_fields_data
        {
            get
            {
                yield return new object[] { "read", 124 };
                yield return new object[] { "Only", 128 };
                yield return new object[] { "hidden", 166 };
                yield return new object[] { "Value", 211 };
                yield return new object[] { "Count", 223 };
                yield return new object[] { "const", 266 };
                yield return new object[] { "what", 305 };
            }
        }

        [Theory, MemberData(nameof(can_find_mistakes_in_various_fields_data))]
        public async Task can_find_mistakes_in_various_fields(string expectedWord, int expectedStart)
        {
            var expectedEnd = expectedStart + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("Fields.SimpleExamples.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(expectedStart, expectedEnd, "Fields.SimpleExamples.cs")
                .And.HaveMessageContaining(expectedWord);
        }

        [Fact]
        public async Task common_prefixes_are_ignored()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("_", "m", "m_"));
            var project = await ReadCodeFileAsProjectAsync("Fields.SimpleExamples.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().BeEmpty();
        }
    }
}
