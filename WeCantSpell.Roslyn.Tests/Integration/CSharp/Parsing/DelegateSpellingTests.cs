using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Roslyn.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class DelegateSpellingTests : CSharpParsingTestBase
    {
        public static object[][] CanFindMistakesInDelegateAndParametersData =>
            new[]
            {
                new object[] { "Do", 5, 22 },
                new object[] { "The", 5, 24 },
                new object[] { "Things", 5, 27 },
                new object[] { "arg", 5, 38 },
                new object[] { "One", 5, 41 },
                new object[] { "param", 5, 53 },
                new object[] { "Two", 5, 58 }
            };

        [Theory, MemberData(nameof(CanFindMistakesInDelegateAndParametersData))]
        public async Task can_find_mistakes_in_delegate_and_parameters(
            string expectedWord,
            int expectedLine,
            int expectedCharacter
        )
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("Delegate.SimpleExamples.csx");

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
                    "Delegate.SimpleExamples.csx"
                )
                .And.HaveMessageContaining(expectedWord);
        }
    }
}
