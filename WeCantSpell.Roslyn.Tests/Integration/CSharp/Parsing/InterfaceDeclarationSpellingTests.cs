using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Roslyn.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class InterfaceDeclarationSpellingTests : CSharpParsingTestBase
    {
        [Fact]
        public async Task interface_name_can_contain_mistakes()
        {
            var analyzer = new SpellingAnalyzerCSharp(
                new WrongWordChecker("Simple", "Interface", "Example")
            );
            var project = await ReadCodeFileAsProjectAsync("TypeName.ISimpleInterfaceExample.csx");

            var diagnostics = (await GetDiagnosticsAsync(project, analyzer)).ToList();

            diagnostics
                .Should()
                .HaveCount(3)
                .And.SatisfyRespectively(
                    first =>
                        first
                            .Should()
                            .HaveMessageContaining("Simple")
                            .And.HaveLineLocation(3, 23, 6, "TypeName.ISimpleInterfaceExample.csx"),
                    second =>
                        second
                            .Should()
                            .HaveMessageContaining("Interface")
                            .And.HaveLineLocation(3, 29, 9, "TypeName.ISimpleInterfaceExample.csx"),
                    third =>
                        third
                            .Should()
                            .HaveMessageContaining("Example")
                            .And.HaveLineLocation(3, 38, 7, "TypeName.ISimpleInterfaceExample.csx")
                );
        }

        [Fact]
        public async Task interface_name_spelling_ignores_prefix_letter()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("I"));
            var project = await ReadCodeFileAsProjectAsync("TypeName.ISimpleInterfaceExample.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().BeEmpty();
        }
    }
}
