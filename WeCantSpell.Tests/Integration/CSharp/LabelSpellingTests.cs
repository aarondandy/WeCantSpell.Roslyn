using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Tests.Integration.CSharp
{
    public class LabelSpellingTests : CSharpTestBase
    {
        public static IEnumerable<object[]> can_find_mistakes_in_labels_data
        {
            get
            {
                yield return new object[] { "go", 150 };
                yield return new object[] { "Wild", 152 };
                yield return new object[] { "jump", 188 };
                yield return new object[] { "Again", 192 };
                yield return new object[] { "strike", 506 };
                yield return new object[] { "Out", 512 };
            }
        }

        [Theory, MemberData(nameof(can_find_mistakes_in_labels_data))]
        public async Task can_find_mistakes_in_labels(string expectedWord, int expectedStart)
        {
            var expectedEnd = expectedStart + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("Label.SimpleExamples.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(expectedStart, expectedEnd, "Label.SimpleExamples.cs")
                .And.HaveMessageContaining(expectedWord);
        }

        [Fact]
        public async Task can_find_mistake_in_labeled_variable_declaration()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("state"));
            var project = await ReadCodeFileAsProjectAsync("Label.SimpleExamples.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(162, 167, "Label.SimpleExamples.cs")
                .And.HaveMessageContaining("state");
        }

        [Fact]
        public async Task does_not_find_mistakes_in_switch_label_keywords()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("case", "default"));
            var project = await ReadCodeFileAsProjectAsync("Label.SimpleExamples.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().BeEmpty();
        }
    }
}
