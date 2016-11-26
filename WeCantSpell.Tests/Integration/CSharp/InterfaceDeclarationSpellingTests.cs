using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Tests.Integration.CSharp
{
    public class InterfaceDeclarationSpellingTests : CSharpTestBase
    {
        [Fact]
        public async Task interface_name_can_contain_mistakes()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("Simple", "Interface", "Example"));
            var project = await ReadCodeFileAsProjectAsync("TypeName.ISimpleInterfaceExample.cs");

            var diagnostics = (await GetDiagnosticsAsync(project, analyzer)).ToList();

            diagnostics.Should().HaveCount(3);
            diagnostics[0].Should().HaveMessageContaining("Simple")
                .And.HaveLocation(79, 85, "TypeName.ISimpleInterfaceExample.cs");
            diagnostics[1].Should().HaveMessageContaining("Interface")
                .And.HaveLocation(85, 94, "TypeName.ISimpleInterfaceExample.cs");
            diagnostics[2].Should().HaveMessageContaining("Example")
                .And.HaveLocation(94, 101, "TypeName.ISimpleInterfaceExample.cs");
        }

        [Fact]
        public async Task interface_name_spelling_ignores_prefix_letter()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("I"));
            var project = await ReadCodeFileAsProjectAsync("TypeName.ISimpleInterfaceExample.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().BeEmpty();
        }
    }
}
