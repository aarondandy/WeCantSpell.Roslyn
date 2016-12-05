using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Tests.Integration.CSharp
{
    public class DelegateSpellingTests : CSharpTestBase
    {
        public static IEnumerable<object[]> can_find_mistakes_in_delegate_and_parameters_data
        {
            get
            {
                yield return new object[] { "Do", 124 };
                yield return new object[] { "The", 126 };
                yield return new object[] { "Things", 129 };
                yield return new object[] { "arg", 140 };
                yield return new object[] { "One", 143 };
                yield return new object[] { "param", 155 };
                yield return new object[] { "Two", 160 };
            }
        }

        [Theory, MemberData(nameof(can_find_mistakes_in_delegate_and_parameters_data))]
        public async Task can_find_mistakes_in_delegate_and_parameters(string expectedWord, int expectedStart)
        {
            var expectedEnd = expectedStart + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("Delegate.SimpleExamples.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(expectedStart, expectedEnd, "Delegate.SimpleExamples.cs")
                .And.HaveMessageContaining(expectedWord);
        }
    }
}
