using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Tests.Integration.CSharp
{
    public class LinqSpellingTests : CSharpTestBase
    {
        public static IEnumerable<object[]> can_find_spelling_mistakes_in_linq_data
        {
            get
            {
                yield return new object[] { "number", 223 };
                yield return new object[] { "odd", 279 };
                yield return new object[] { "intermediary", 371 };
                yield return new object[] { "Max", 430 };
                yield return new object[] { "Value", 465 };
                yield return new object[] { "group", 560 };
                yield return new object[] { "Odd", 681 };
                yield return new object[] { "Values", 720 };
                yield return new object[] { "letterint", 820 };
                yield return new object[] { "potato", 943 };
                yield return new object[] { "goat", 993 };
                yield return new object[] { "apple", 1047 };
                yield return new object[] { "basket", 1101 };
                yield return new object[] { "Category", 1206 };
                yield return new object[] { "Produce", 1260 };
            }
        }

        [Theory, MemberData(nameof(can_find_spelling_mistakes_in_linq_data))]
        public async Task can_find_spelling_mistakes_in_linq(string expectedWord, int expectedStart)
        {
            var expectedEnd = expectedStart + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("Linq.SimpleExamples.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLocation(expectedStart, expectedEnd, "Linq.SimpleExamples.cs")
                .And.HaveMessageContaining(expectedWord);
        }
    }
}
