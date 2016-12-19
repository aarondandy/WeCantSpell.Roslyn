using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Tests.Integration.CSharp
{
    public class StringLiteralSpellingTests : CSharpTestBase
    {
        public static IEnumerable<object[]> can_find_mistakes_in_literals_data
        {
            get
            {
                yield return new object[] { "apple", 156 };
                yield return new object[] { "banana", 162 };
                yield return new object[] { "cranberry", 169 };
                yield return new object[] { "dragon-fruit", 248 };
                yield return new object[] { "edamame", 262 };
                yield return new object[] { "fig", 270 };
                yield return new object[] { "gooseberry", 310 };
                yield return new object[] { "huckleberry", 323 };
                yield return new object[] { "イチゴ", 337 };
                yield return new object[] { "jackfruit", 412 };
                yield return new object[] { "kiwi", 427 };
                yield return new object[] { "lemon", 436 };
                yield return new object[] { "mango", 453 };
                yield return new object[] { "nectarine", 462 };
                yield return new object[] { "orange", 474 };
                yield return new object[] { "papaya", 482 };
                yield return new object[] { "quince", 492 };
                yield return new object[] { "raspberry", 502 };
                yield return new object[] { "strawberry", 557 };
                yield return new object[] { "shake", 570 };
                yield return new object[] { "tomato", 614 };
                yield return new object[] { "ugli", 622 };
            }
        }

        [Theory, MemberData(nameof(can_find_mistakes_in_literals_data))]
        public async Task can_find_mistakes_in_literals(string expectedWord, int expectedStart)
        {
            var expectedEnd = expectedStart + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("StringLiteral.SimpleExamples.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3111")
                .And.HaveLocation(expectedStart, expectedEnd, "StringLiteral.SimpleExamples.cs")
                .And.HaveMessageContaining(expectedWord);
        }
    }
}
