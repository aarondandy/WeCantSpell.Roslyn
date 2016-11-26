using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Tests.Integration.CSharp
{
    public class PropertyDeclarationSpellingTests : CSharpTestBase
    {
        public static IEnumerable<object[]> can_find_mistakes_in_various_properties_data
        {
            get
            {
                yield return new object[] { "Read", 133 };
                yield return new object[] { "Only", 137 };
                yield return new object[] { "Generated", 176 };
                yield return new object[] { "Backing", 185 };
                yield return new object[] { "Hand", 232 };
                yield return new object[] { "Made", 236 };
                yield return new object[] { "Uuid", 429 };
            }
        }

        [Theory, MemberData(nameof(can_find_mistakes_in_various_properties_data))]
        public async Task can_find_mistakes_in_various_properties(string expectedWord, int expectedStart)
        {
            var expectedEnd = expectedStart + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("Properties.SimpleExamples.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(expectedStart, expectedEnd, "Properties.SimpleExamples.cs")
                .And.HaveMessageContaining(expectedWord);
        }

        [Fact]
        public async Task can_find_mistakes_in_struct_props()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("Count"));
            var project = await ReadCodeFileAsProjectAsync("TypeName.SimpleStructExample.cs");

            var diagnostics = (await GetDiagnosticsAsync(project, analyzer)).ToList();

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(154, 159, "TypeName.SimpleStructExample.cs")
                .And.HaveMessageContaining("Count");
        }

        [Fact]
        public async Task can_find_mistakes_in_interface_props()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("Count"));
            var project = await ReadCodeFileAsProjectAsync("TypeName.ISimpleInterfaceExample.cs");

            var diagnostics = (await GetDiagnosticsAsync(project, analyzer)).ToList();

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(122, 127, "TypeName.ISimpleInterfaceExample.cs")
                .And.HaveMessageContaining("Count");
        }
    }
}
