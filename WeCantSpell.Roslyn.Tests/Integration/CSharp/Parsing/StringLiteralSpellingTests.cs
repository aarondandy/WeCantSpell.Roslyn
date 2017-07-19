using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Roslyn.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class StringLiteralSpellingTests : CSharpParsingTestBase
    {
        public static object[][] can_find_mistakes_in_literals_data => new[]
        {
            new object[] { "apple", 156 },
            new object[] { "banana", 162 },
            new object[] { "cranberry", 169 },
            new object[] { "dragon-fruit", 248 },
            new object[] { "edamame", 262 },
            new object[] { "fig", 270 },
            new object[] { "gooseberry", 310 },
            new object[] { "huckleberry", 323 },
            new object[] { "イチゴ", 337 },
            new object[] { "jackfruit", 412 },
            new object[] { "kiwi", 427 },
            new object[] { "lemon", 436 },
            new object[] { "mango", 453 },
            new object[] { "nectarine", 462 },
            new object[] { "orange", 474 },
            new object[] { "papaya", 482 },
            new object[] { "quince", 492 },
            new object[] { "raspberry", 502 },
            new object[] { "strawberry", 557 },
            new object[] { "shake", 570 },
            new object[] { "tomato", 614 },
            new object[] { "ugli", 622 }
        };

        [Theory, MemberData(nameof(can_find_mistakes_in_literals_data))]
        public async Task can_find_mistakes_in_literals(string expectedWord, int expectedStart)
        {
            var expectedEnd = expectedStart + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("StringLiteral.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3111")
                .And.HaveLocation(expectedStart, expectedEnd, "StringLiteral.SimpleExamples.csx")
                .And.HaveMessageContaining(expectedWord);
        }
    }
}
