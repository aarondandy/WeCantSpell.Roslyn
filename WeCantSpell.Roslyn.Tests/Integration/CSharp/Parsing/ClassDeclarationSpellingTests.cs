using System.Linq;
using System.Threading.Tasks;
using WeCantSpell.Roslyn.Tests.Utilities;

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

            diagnostics
                .Should()
                .ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLineLocation(3, 18, 5, "TypeName.FirstMiddleLast.csx")
                .And.HaveMessageContaining("First");
        }

        [Fact]
        public async Task name_ends_with_spelling_mistake()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("Last"));
            var project = await ReadCodeFileAsProjectAsync("TypeName.FirstMiddleLast.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics
                .Should()
                .ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLineLocation(3, 29, 4, "TypeName.FirstMiddleLast.csx")
                .And.HaveMessageContaining("Last");
        }

        [Fact]
        public async Task name_contains_spelling_mistake_in_the_middle()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("Middle"));
            var project = await ReadCodeFileAsProjectAsync("TypeName.FirstMiddleLast.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics
                .Should()
                .ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLineLocation(3, 23, 6, "TypeName.FirstMiddleLast.csx")
                .And.HaveMessageContaining("Middle");
        }

        [Fact]
        public async Task name_contains_individual_mistakes_for_all_words()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("First", "Middle", "Last"));
            var project = await ReadCodeFileAsProjectAsync("TypeName.FirstMiddleLast.csx");

            var diagnostics = (await GetDiagnosticsAsync(project, analyzer)).ToList();

            diagnostics
                .Should()
                .HaveCount(3)
                .And.SatisfyRespectively(
                    first =>
                        first
                            .Should()
                            .HaveMessageContaining("First")
                            .And.HaveLineLocation(3, 18, 5, "TypeName.FirstMiddleLast.csx"),
                    second =>
                        second
                            .Should()
                            .HaveMessageContaining("Middle")
                            .And.HaveLineLocation(3, 23, 6, "TypeName.FirstMiddleLast.csx"),
                    third =>
                        third
                            .Should()
                            .HaveMessageContaining("Last")
                            .And.HaveLineLocation(3, 29, 4, "TypeName.FirstMiddleLast.csx")
                );
        }
    }
}
