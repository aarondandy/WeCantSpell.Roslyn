using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Tests.Integration.CSharp
{
    public class EnumDeclarationSpellingTests : CSharpTestBase
    {
        public static IEnumerable<object[]> can_find_mistakes_in_enums_data
        {
            get
            {
                yield return new object[] { "Example", 73 };
                yield return new object[] { "Value", 101 };
                yield return new object[] { "One", 106 };
                yield return new object[] { "Number", 124 };
                yield return new object[] { "Two", 130 };
            }
        }

        [Theory, MemberData(nameof(can_find_mistakes_in_enums_data))]
        public async Task can_find_mistakes_in_enums(string expectedWord, int expectedStart)
        {
            var expectedEnd = expectedStart + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("Enum.SimpleExamples.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(expectedStart, expectedEnd, "Enum.SimpleExamples.cs")
                .And.HaveMessageContaining(expectedWord);
        }
    }
}
