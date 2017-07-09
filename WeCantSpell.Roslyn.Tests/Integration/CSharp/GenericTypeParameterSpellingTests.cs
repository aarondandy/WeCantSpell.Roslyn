using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Roslyn.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp
{
    public class GenericTypeParameterSpellingTests : CSharpTestBase
    {
        public static object[][] can_find_mistakes_in_generic_parameter_names_data => new[]
        {
            new object[] { "In", 99 },
            new object[] { "Out", 108 },
            new object[] { "Struct", 203 },
            new object[] { "Thing", 321 },
            new object[] { "Gadget", 368 },
            new object[] { "Ion", 448 }
        };

        [Theory, MemberData(nameof(can_find_mistakes_in_generic_parameter_names_data))]
        public async Task can_find_mistakes_in_generic_parameter_names(string expectedWord, int expectedStart)
        {
            var expectedEnd = expectedStart + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("GenericType.SimpleExamples.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(expectedStart, expectedEnd, "GenericType.SimpleExamples.cs")
                .And.HaveMessageContaining(expectedWord);
        }

        [Fact]
        public async Task generic_parameter_prefix_is_ignored()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("T"));
            var project = await ReadCodeFileAsProjectAsync("GenericType.SimpleExamples.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().BeEmpty();
        }
    }
}
