using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Roslyn.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class MethodDeclarationSpellingTests : CSharpParsingTestBase
    {
        public static object[][] can_find_mistakes_in_methods_data => new[]
        {
            new object[] { "STATIC", 125 },
            new object[] { "METHOD", 132 },
            new object[] { "set", 215 },
            new object[] { "Timeout", 218 },
            new object[] { "Internal", 322 },
            new object[] { "By", 373 },
            new object[] { "Ref", 375 }
        };

        [Theory, MemberData(nameof(can_find_mistakes_in_methods_data))]
        public async Task can_find_mistakes_in_methods(string expectedWord, int expectedStart)
        {
            var expectedEnd = expectedStart + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("MethodNames.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(expectedStart, expectedEnd, "MethodNames.SimpleExamples.csx")
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

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(197, 203, "TypeName.SimpleStructExample.csx")
                .And.HaveMessageContaining("Method");
        }

        [Fact]
        public async Task can_find_mistake_in_interface()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("Method"));
            var project = await ReadCodeFileAsProjectAsync("TypeName.ISimpleInterfaceExample.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(153, 159, "TypeName.ISimpleInterfaceExample.csx")
                .And.HaveMessageContaining("Method");
        }

        [Fact]
        public async Task operators_are_ignored()
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker("operator", "+", "operator+", "string", "Guid", "System.Guid", "op_Addition"));
            var project = await ReadCodeFileAsProjectAsync("MethodNames.OperatorExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().BeEmpty();
        }
    }
}
