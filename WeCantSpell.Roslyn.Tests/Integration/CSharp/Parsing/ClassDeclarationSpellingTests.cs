using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Roslyn.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class ClassDeclarationSpellingTests : CSharpParsingTestBase
    {
        [Fact]
        public async Task name_has_no_spelling_mistakes()
        {
            var analyzer = new SpellingAnalyzerCSharp(new AllGoodWordChecker());
            var project = await ReadCodeFileAsProjectAsync("TypeName.FirstMiddleLast.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().BeEmpty();
        }

        [Fact]
        public async Task name_start_with_spelling_mistake()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("First"));
            var project = await ReadCodeFileAsProjectAsync("TypeName.FirstMiddleLast.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(74, 79, "TypeName.FirstMiddleLast.csx")
                .And.HaveMessageContaining("First");
        }

        [Fact]
        public async Task name_ends_with_spelling_mistake()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("Last"));
            var project = await ReadCodeFileAsProjectAsync("TypeName.FirstMiddleLast.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(85, 89, "TypeName.FirstMiddleLast.csx")
                .And.HaveMessageContaining("Last");
        }

        [Fact]
        public async Task name_contains_spelling_mistake_in_the_middle()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("Middle"));
            var project = await ReadCodeFileAsProjectAsync("TypeName.FirstMiddleLast.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(79, 85, "TypeName.FirstMiddleLast.csx")
                .And.HaveMessageContaining("Middle");
        }

        [Fact]
        public async Task name_contains_individual_mistakes_for_all_words()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("First", "Middle", "Last"));
            var project = await ReadCodeFileAsProjectAsync("TypeName.FirstMiddleLast.csx");

            var diagnostics = (await GetDiagnosticsAsync(project, analyzer)).ToList();

            diagnostics.Should().HaveCount(3);
            diagnostics[0].Should().HaveMessageContaining("First")
                .And.HaveLocation(74, 79, "TypeName.FirstMiddleLast.csx");
            diagnostics[1].Should().HaveMessageContaining("Middle")
                .And.HaveLocation(79, 85, "TypeName.FirstMiddleLast.csx");
            diagnostics[2].Should().HaveMessageContaining("Last")
                .And.HaveLocation(85, 89, "TypeName.FirstMiddleLast.csx");
        }
    }
}
