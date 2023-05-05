using System.Linq;
using FluentAssertions;
using Xunit;

namespace WeCantSpell.Roslyn.Tests
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
        [InlineData(" // x ", 4, 1)]
        [InlineData("", 0, 0)]
        public void CanExtractCommentText(string text, int expectedStart, int expectedLength)
        {
            var result = CommentTextExtractor.LocateSingleLineCommentText(text);

            result.Start.Should().Be(expectedStart);
            result.Length.Should().Be(expectedLength);
        }

        [Fact]
        public void CanExtractMultilineParts()
        {
            const string text = "    /*\r\n     * Words words words.\r\n     * This is aardvark.\r\n\r\n   * \r\n     * <code> \r     * Console.WriteLine(\"code\");\r\n     * </code>\r\n     */";

            var parts = CommentTextExtractor.LocateMultiLineCommentTextParts(text).ToList();

            parts.Should().HaveCount(5);
            parts[0].Start.Should().Be(15);
            parts[0].Length.Should().Be(18);
            parts[1].Start.Should().Be(42);
            parts[1].Length.Should().Be(17);
            parts[2].Start.Should().Be(77);
            parts[2].Length.Should().Be(6);
            parts[3].Start.Should().Be(92);
            parts[3].Length.Should().Be(26);
            parts[4].Start.Should().Be(127);
            parts[4].Length.Should().Be(7);
        }

        [Fact]
        public void TightMultilineBlockCanReadAllLines()
        {
            const string text = "/* line one\n   line two\n   line three */";

            var parts = CommentTextExtractor.LocateMultiLineCommentTextParts(text).ToList();

            parts.Should().HaveCount(3);
            parts[0].Start.Should().Be(3);
            parts[0].Length.Should().Be(8);
            parts[1].Start.Should().Be(15);
            parts[1].Length.Should().Be(8);
            parts[2].Start.Should().Be(27);
            parts[2].Length.Should().Be(10);
        }

        [Fact]
        public void SinleLinedMultilineBlockCanReadAllLines()
        {
            const string text = " /* line one */ ";

            var parts = CommentTextExtractor.LocateMultiLineCommentTextParts(text);

            parts.Should().HaveCount(1);
            parts.Single().Start.Should().Be(4);
            parts.Single().Length.Should().Be(8);
        }
    }
}
