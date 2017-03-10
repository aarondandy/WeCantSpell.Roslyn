using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using WeCantSpell.Tests.Utilities;
using Xunit;

namespace WeCantSpell.Tests.Integration.CSharp
{
    public class CommentSpellingTests : CSharpTestBase
    {
        public static IEnumerable<object[]> can_find_mistakes_in_comments_data
        {
            get
            {
                yield return new object[] { "aardvark", 660 };
                yield return new object[] { "simple", 1186 };
                yield return new object[] { "under", 1235 };
                yield return new object[] { "inline", 111 };
                yield return new object[] { "tag", 320 };
                yield return new object[] { "Here", 898 };
                yield return new object[] { "Just", 1130 };
            }
        }

        [Theory, MemberData(nameof(can_find_mistakes_in_comments_data))]
        public async Task can_find_mistakes_in_comments(string expectedWord, int expectedStart)
        {
            var expectedEnd = expectedStart + expectedWord.Length;

            var analyzer = new SpellingAnalyzerCSharp(new WrongWordChecker(expectedWord));
            var project = await ReadCodeFileAsProjectAsync("XmlDoc.SimpleExamples.cs");

            var diagnostics = await GetDiagnosticsAsync(project, analyzer);

            diagnostics.Should().ContainSingle()
                .Subject.Should()
                .HaveId("SP3112")
                .And.HaveLocation(expectedStart, expectedEnd, "XmlDoc.SimpleExamples.cs")
                .And.HaveMessageContaining(expectedWord);
        }
    }
}
