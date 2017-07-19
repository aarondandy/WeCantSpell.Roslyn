using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Roslyn.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class DelegateSpellingTests : CSharpParsingTestBase
    {
        public static object[][] can_find_mistakes_in_delegate_and_parameters_data => new[]
        {
            new object[] { "Do", 124 },
            new object[] { "The", 126 },
            new object[] { "Things", 129 },
            new object[] { "arg", 140 },
            new object[] { "One", 143 },
            new object[] { "param", 155 },
            new object[] { "Two", 160 }
        };

        [Theory, MemberData(nameof(can_find_mistakes_in_delegate_and_parameters_data))]
        public async Task can_find_mistakes_in_delegate_and_parameters(string expectedWord, int expectedStart)
        {
            var expectedEnd = expectedStart + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("Delegate.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(expectedStart, expectedEnd, "Delegate.SimpleExamples.csx")
                .And.HaveMessageContaining(expectedWord);
        }
    }
}
