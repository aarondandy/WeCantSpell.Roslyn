using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Roslyn.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class GenericTypeParameterSpellingTests : CSharpParsingTestBase
    {
        public static object[][] CanFindMistakesInGenericParameterNamesData => new[]
        {
            new object[] { "In", 3, 43 },
            new object[] { "Out", 3, 52 },
            new object[] { "Struct", 9, 38 },
            new object[] { "Thing", 14, 32 },
            new object[] { "Gadget", 16, 33 },
            new object[] { "Ion", 18, 36 }
        };

        [Theory, MemberData(nameof(CanFindMistakesInGenericParameterNamesData))]
        public async Task can_find_mistakes_in_generic_parameter_names(string expectedWord, int expectedLine,
            int expectedCharacter
        )
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("GenericType.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLineLocation(expectedLine, expectedCharacter, expectedWord.Length, "GenericType.SimpleExamples.csx")
                .And.HaveMessageContaining(expectedWord);
        }

        [Fact]
        public async Task generic_parameter_prefix_is_ignored()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("T"));
            var project = await ReadCodeFileAsProjectAsync("GenericType.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().BeEmpty();
        }
    }
}
