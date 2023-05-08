using System.Linq;
using System.Threading.Tasks;
using WeCantSpell.Roslyn.Tests.Utilities;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class ParameterSpellingTests : CSharpParsingTestBase
    {
        public static object[][] CanFindMistakesInMethodParameterNamesData =>
            new[]
            {
                new object[] { "number", 5, 48 },
                new object[] { "many", 5, 63 },
                new object[] { "Words", 5, 67 },
                new object[] { "count", 9, 35 },
                new object[] { "name", 9, 49 },
                new object[] { "uuid", 16, 43 },
                new object[] { "result", 16, 60 }
            };

        [Theory, MemberData(nameof(CanFindMistakesInMethodParameterNamesData))]
        public async Task can_find_mistakes_in_method_parameter_names(
            string expectedWord,
            int expectedLine,
            int expectedCharacter
        )
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("MethodNames.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics
                .Should()
                .ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLineLocation(
                    expectedLine,
                    expectedCharacter,
                    expectedWord.Length,
                    "MethodNames.SimpleExamples.csx"
                )
                .And.HaveMessageContaining(expectedWord);
        }

        public static object[][] CanFindMistakesInLambdaParametersData =>
            new[]
            {
                new object[] { "word", 10, 38 },
                new object[] { "count", 11, 50 },
                new object[] { "Things", 11, 57 },
                new object[] { "value", 11, 65 },
                new object[] { "number", 12, 50 }
            };

        [Theory, MemberData(nameof(CanFindMistakesInLambdaParametersData))]
        public async Task can_find_mistakes_in_lambda_parameters(
            string expectedWord,
            int expectedLine,
            int expectedCharacter
        )
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("Lambda.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics
                .Should()
                .ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLineLocation(expectedLine, expectedCharacter, expectedWord.Length, "Lambda.SimpleExamples.csx")
                .And.HaveMessageContaining(expectedWord);
        }

        [Fact]
        public async Task can_find_mistakes_in_indexer_params()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("index", "word"));
            var project = await ReadCodeFileAsProjectAsync("Properties.SimpleExamples.csx");

            var diagnostics = (await GetDiagnosticsAsync(project, analyzer)).ToList();

            diagnostics
                .Should()
                .HaveCount(2)
                .And.SatisfyRespectively(
                    first =>
                        first
                            .Should()
                            .HaveMessageContaining("index")
                            .And.HaveLineLocation(25, 29, 5, "Properties.SimpleExamples.csx"),
                    second =>
                        second
                            .Should()
                            .HaveMessageContaining("word")
                            .And.HaveLineLocation(25, 43, 4, "Properties.SimpleExamples.csx")
                );
        }
    }
}
