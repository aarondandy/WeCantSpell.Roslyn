using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Roslyn.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class LabelSpellingTests : CSharpParsingTestBase
    {
        public static object[][] can_find_mistakes_in_labels_data => new[]
        {
            new object[] { "go", 150 },
            new object[] { "Wild", 152 },
            new object[] { "jump", 188 },
            new object[] { "Again", 192 },
            new object[] { "strike", 506 },
            new object[] { "Out", 512 }
        };

        [Theory, MemberData(nameof(can_find_mistakes_in_labels_data))]
        public async Task can_find_mistakes_in_labels(string expectedWord, int expectedStart)
        {
            var expectedEnd = expectedStart + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("Label.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(expectedStart, expectedEnd, "Label.SimpleExamples.csx")
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
                .And.HaveLocation(162, 167, "Label.SimpleExamples.csx")
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
