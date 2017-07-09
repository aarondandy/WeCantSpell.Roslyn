using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Roslyn.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp
{
    public class StructDeclarationSpellingTests : CSharpTestBase
    {
        [Fact]
        public async Task struct_name_can_contain_mistakes()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("Simple", "Struct", "Example"));
            var project = await ReadCodeFileAsProjectAsync("TypeName.SimpleStructExample.cs");

            var diagnostics = (await GetDiagnosticsAsync(project, analyzer)).ToList();

            diagnostics.Should().HaveCount(3);
            diagnostics[0].Should().HaveMessageContaining("Simple")
                .And.HaveLocation(75, 81, "TypeName.SimpleStructExample.cs");
            diagnostics[1].Should().HaveMessageContaining("Struct")
                .And.HaveLocation(81, 87, "TypeName.SimpleStructExample.cs");
            diagnostics[2].Should().HaveMessageContaining("Example")
                .And.HaveLocation(87, 94, "TypeName.SimpleStructExample.cs");
        }
    }
}
