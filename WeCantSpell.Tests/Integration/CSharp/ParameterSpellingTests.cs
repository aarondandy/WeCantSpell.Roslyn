using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Tests.Integration.CSharp
{
    public class ParameterSpellingTests : CSharpTestBase
    {
        public static object[][] can_find_mistakes_in_method_parameter_names_data => new[]
        {
            new object[] { "number", 145 },
            new object[] { "many", 160 },
            new object[] { "Words", 164 },
            new object[] { "count", 230 },
            new object[] { "name", 244 },
            new object[] { "uuid", 395 },
            new object[] { "result", 412 }
        };

        [Theory, MemberData(nameof(can_find_mistakes_in_method_parameter_names_data))]
        public async Task can_find_mistakes_in_method_parameter_names(string expectedWord, int expectedStart)
        {
            var expectedEnd = expectedStart + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("MethodNames.SimpleExamples.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(expectedStart, expectedEnd, "MethodNames.SimpleExamples.cs")
                .And.HaveMessageContaining(expectedWord);
        }

        public static object[][] can_find_mistakes_in_lambda_parameters_data => new[]
        {
            new object[] { "word", 227 },
            new object[] { "count", 305 },
            new object[] { "Things", 312 },
            new object[] { "value", 320 },
            new object[] { "number", 417 }
        };

        [Theory, MemberData(nameof(can_find_mistakes_in_lambda_parameters_data))]
        public async Task can_find_mistakes_in_lambda_parameters(string expectedWord, int expectedStart)
        {
            var expectedEnd = expectedStart + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("Lambda.SimpleExamples.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(expectedStart, expectedEnd, "Lambda.SimpleExamples.cs")
                .And.HaveMessageContaining(expectedWord);
        }

        [Fact]
        public async Task can_find_mistakes_in_indexer_params()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("index", "word"));
            var project = await ReadCodeFileAsProjectAsync("Properties.SimpleExamples.cs");

            var diagnostics = (await GetDiagnosticsAsync(project, analyzer)).ToList();

            diagnostics.Should().HaveCount(2);
            diagnostics[0].Should().HaveMessageContaining("index")
                .And.HaveLocation(484, 489, "Properties.SimpleExamples.cs");
            diagnostics[1].Should().HaveMessageContaining("word")
                .And.HaveLocation(498, 502, "Properties.SimpleExamples.cs");
        }
    }
}
