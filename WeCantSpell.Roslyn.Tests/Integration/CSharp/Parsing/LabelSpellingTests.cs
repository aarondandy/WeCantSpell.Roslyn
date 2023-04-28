using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Roslyn.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class LabelSpellingTests : CSharpParsingTestBase
    {
        public static object[][] CanFindMistakesInLabelsData => new[]
        {
            new object[] { "go", 7, 13 },
            new object[] { "Wild", 7, 15 },
            new object[] { "jump", 9, 13 },
            new object[] { "Again", 9, 17 },
            new object[] { "strike", 25, 13 },
            new object[] { "Out", 25, 19 }
        };

        [Theory, MemberData(nameof(CanFindMistakesInLabelsData))]
        public async Task can_find_mistakes_in_labels(string expectedWord, int expectedStart, int expectedCharacter)
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("Label.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLineLocation(expectedStart, expectedCharacter, expectedWord.Length, "Label.SimpleExamples.csx")
                .And.HaveMessageContaining(expectedWord);
        }

        [Fact]
        public async Task can_find_mistake_in_labeled_variable_declaration()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("state"));
            var project = await ReadCodeFileAsProjectAsync("Label.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLineLocation(7,25, 5, "Label.SimpleExamples.csx")
                .And.HaveMessageContaining("state");
        }

        [Fact]
        public async Task does_not_find_mistakes_in_switch_label_keywords()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("case", "default"));
            var project = await ReadCodeFileAsProjectAsync("Label.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().BeEmpty();
        }
    }
}
