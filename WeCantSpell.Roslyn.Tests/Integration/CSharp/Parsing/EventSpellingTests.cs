using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Roslyn.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public class EventSpellingTests : CSharpParsingTestBase
    {
        public static IEnumerable<object[]> CanFindMistakesInVariousFieldsData => new[]
        {
            new object[] { "Do", 5, 43 },
            new object[] { "The", 5, 45 },
            new object[] { "Thing", 5, 48 },
            new object[] { "Click", 7, 36 },
            new object[] { "Clack", 7, 41 }
        };

        [Theory, MemberData(nameof(CanFindMistakesInVariousFieldsData))]
        public async Task can_find_mistakes_in_various_fields(string expectedWord, int expectedLine,
            int expectedCharacter
        )
        {
            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("Events.SimpleExamples.csx");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3110")
                .And.HaveLineLocation(expectedLine, expectedCharacter, expectedWord.Length, "Events.SimpleExamples.csx")
                .And.HaveMessageContaining(expectedWord);
        }
    }
}
