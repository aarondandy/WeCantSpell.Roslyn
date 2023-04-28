using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Roslyn.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class StructDeclarationSpellingTests : CSharpParsingTestBase
    {
        [Fact]
        public async Task struct_name_can_contain_mistakes()
        {
            var analyzer = new SpellingAnalyzerCSharp(
                new WrongWordChecker("Simple", "Struct", "Example")
            );
            var project = await ReadCodeFileAsProjectAsync("TypeName.SimpleStructExample.csx");

            var diagnostics = (await GetDiagnosticsAsync(project, analyzer)).ToList();

            diagnostics
                .Should()
                .HaveCount(3)
                .And.SatisfyRespectively(
                    first =>
                        first
                            .Should()
                            .HaveMessageContaining("Simple")
                            .And.HaveLineLocation(3, 19, 6, "TypeName.SimpleStructExample.csx"),
                    second =>
                        second
                            .Should()
                            .HaveMessageContaining("Struct")
                            .And.HaveLineLocation(3, 25, 6, "TypeName.SimpleStructExample.csx"),
                    third =>
                        third
                            .Should()
                            .HaveMessageContaining("Example")
                            .And.HaveLineLocation(3, 31, 7, "TypeName.SimpleStructExample.csx")
                );
        }
    }
}
