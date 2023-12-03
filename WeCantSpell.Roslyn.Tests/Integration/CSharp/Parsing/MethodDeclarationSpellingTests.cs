using System.Threading.Tasks;
using WeCantSpell.Roslyn.Tests.Utilities;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class MethodDeclarationSpellingTests : CSharpParsingTestBase
    {
        public static object[][] CanFindMistakesInMethodsData =>
            new[]
            {
                new object[] { "STATIC", 5, 28 },
                new object[] { "METHOD", 5, 35 },
                new object[] { "set", 9, 20 },
                new object[] { "Timeout", 9, 23 },
                new object[] { "Internal", 14, 25 },
                new object[] { "By", 16, 21 },
                new object[] { "Ref", 16, 23 }
            };

        [Theory, MemberData(nameof(CanFindMistakesInMethodsData))]
        public async Task can_find_mistakes_in_methods(string expectedWord, int expectedLine, int expectedCharacter)
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

        [Fact]
        public async Task does_not_find_mistakes_for_invocation()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("System", "Console", "Write", "Line"));
            var project = await ReadCodeFileAsProjectAsync("MethodNames.Invocation.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().BeEmpty();
        }

        [Fact]
        public async Task can_find_mistake_in_struct()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("Method"));
            var project = await ReadCodeFileAsProjectAsync("TypeName.SimpleStructExample.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics
                .Should()
                .ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLineLocation(9, 21, 6, "TypeName.SimpleStructExample.csx")
                .And.HaveMessageContaining("Method");
        }

        [Fact]
        public async Task can_find_mistake_in_interface()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("Method"));
            var project = await ReadCodeFileAsProjectAsync("TypeName.ISimpleInterfaceExample.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics
                .Should()
                .ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLineLocation(7, 14, 6, "TypeName.ISimpleInterfaceExample.csx")
                .And.HaveMessageContaining("Method");
        }

        [Fact]
        public async Task operators_are_ignored()
        {
            var analyzer = new SpellingAnalyzerCSharp(
                new WrongWordChecker("operator", "+", "operator+", "string", "Guid", "System.Guid", "op_Addition")
            );
            var project = await ReadCodeFileAsProjectAsync("MethodNames.OperatorExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().BeEmpty();
        }
    }
}
