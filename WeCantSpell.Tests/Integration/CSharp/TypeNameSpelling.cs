using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Tests.Integration.CSharp
{
    public class TypeNameSpelling : CSharpTestBase
    {
        [Fact]
        public async Task name_has_no_spelling_mistakes()
        {
            var analyzer = new SpellingAnalyzerCSharp(new AllGoodWordChecker());
            var project = await ReadCodeFileAsProjectAsync("TypeName.FirstMiddleLast.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().BeEmpty();
        }

        [Fact]
        public async Task name_start_with_spelling_mistake()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("First"));
            var project = await ReadCodeFileAsProjectAsync("TypeName.FirstMiddleLast.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(74, 79, "TypeName.FirstMiddleLast.cs")
                .And.HaveMessageContaining("First");
        }

        [Fact]
        public async Task name_ends_with_spelling_mistake()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("Last"));
            var project = await ReadCodeFileAsProjectAsync("TypeName.FirstMiddleLast.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(85, 89, "TypeName.FirstMiddleLast.cs")
                .And.HaveMessageContaining("Last");
        }

        [Fact]
        public async Task name_contains_spelling_mistake_in_the_middle()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("Middle"));
            var project = await ReadCodeFileAsProjectAsync("TypeName.FirstMiddleLast.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(79, 85, "TypeName.FirstMiddleLast.cs")
                .And.HaveMessageContaining("Middle");
        }
    }
}
