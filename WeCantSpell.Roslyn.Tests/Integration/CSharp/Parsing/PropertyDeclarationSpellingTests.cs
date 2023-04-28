using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Roslyn.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class PropertyDeclarationSpellingTests : CSharpParsingTestBase
    {
        public static object[][] CanFindMistakesInVariousPropertiesData => new[]
        {
            new object[] { "Read", 7, 20 },
            new object[] { "Only", 7, 24 },
            new object[] { "Generated", 9, 23 },
            new object[] { "Backing", 9, 32 },
            new object[] { "Hand", 11, 23 },
            new object[] { "Made", 11, 27 },
            new object[] { "Uuid", 23, 22 }
        };

        [Theory, MemberData(nameof(CanFindMistakesInVariousPropertiesData))]
        public async Task can_find_mistakes_in_various_properties(string expectedWord, int expectedLine,
            int expectedCharacter
        )
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("Properties.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLineLocation(expectedLine, expectedCharacter, expectedWord.Length, "Properties.SimpleExamples.csx")
                .And.HaveMessageContaining(expectedWord);
        }

        [Fact]
        public async Task can_find_mistakes_in_struct_props()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("Count"));
            var project = await ReadCodeFileAsProjectAsync("TypeName.SimpleStructExample.csx");

            var diagnostics = (await GetDiagnosticsAsync(project, analyzer)).ToList();

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLineLocation(7, 20,5, "TypeName.SimpleStructExample.csx")
                .And.HaveMessageContaining("Count");
        }

        [Fact]
        public async Task can_find_mistakes_in_interface_props()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("Count"));
            var project = await ReadCodeFileAsProjectAsync("TypeName.ISimpleInterfaceExample.csx");

            var diagnostics = (await GetDiagnosticsAsync(project, analyzer)).ToList();

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLineLocation(5, 13, 5, "TypeName.ISimpleInterfaceExample.csx")
                .And.HaveMessageContaining("Count");
        }
    }
}
