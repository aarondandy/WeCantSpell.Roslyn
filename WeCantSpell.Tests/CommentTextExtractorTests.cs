using FluentAssertions;
using Xunit;

namespace WeCantSpell.Tests
{
    public class CommentTextExtractorTests
    {
        [Theory]
        [InlineData("// comment", 3, 7)]
        [InlineData("// comment     ", 3, 7)]
        [InlineData("  // comment", 5, 7)]
        [InlineData("  // comment   ", 5, 7)]
        [InlineData("   // words words words   ", 6, 17)]
        [InlineData("   // a   ", 6, 1)]
        [InlineData("   // ", 6, 0)]
        [InlineData("//", 2, 0)]
        public void CanExtractCommentText(string text, int expectedStart, int expectedLength)
        {
            var result = CommentTextExtractor.LocateSingleLineCommentText(text);

            result.Start.Should().Be(expectedStart);
            result.Length.Should().Be(expectedLength);
        }
    }
}
