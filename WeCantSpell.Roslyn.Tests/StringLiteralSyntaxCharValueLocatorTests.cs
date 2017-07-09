using FluentAssertions;
using Xunit;

namespace WeCantSpell.Roslyn.Tests
{
    public class StringLiteralSyntaxCharValueLocatorTests
    {

        [Theory]
        [InlineData("abc", "\"abc\"", false, 0, 1)]
        [InlineData("abc", "\"abc\"", false, 1, 2)]
        [InlineData("abc", "\"abc\"", false, 2, 3)]
        [InlineData("abc", "@\"abc\"", true, 0, 2)]
        [InlineData("abc", "@\"abc\"", true, 1, 3)]
        [InlineData("abc", "@\"abc\"", true, 2, 4)]
        [InlineData("a\tb\nc", "\"a\\tb\\nc\"", false, 0, 1)]
        [InlineData("a\tb\nc", "\"a\\tb\\nc\"", false, 1, 2)]
        [InlineData("a\tb\nc", "\"a\\tb\\nc\"", false, 2, 4)]
        [InlineData("a\tb\nc", "\"a\\tb\\nc\"", false, 3, 5)]
        [InlineData("a\tb\nc", "\"a\\tb\\nc\"", false, 4, 7)]
        [InlineData("abc", "\"a\\u0062c\"", false, 0, 1)]
        [InlineData("abc", "\"a\\u0062c\"", false, 1, 2)]
        [InlineData("abc", "\"a\\u0062c\"", false, 2, 8)]
        [InlineData("a\xax", "\"a\\xax\"", false, 2, 5)]
        [InlineData("a\x62x", "\"a\\x62x\"", false, 2, 6)]
        [InlineData("a\x062x", "\"a\\x062x\"", false, 2, 7)]
        [InlineData("a\x0062x", "\"a\\x0062x\"", false, 2, 8)]
        [InlineData("\"abc\"", "@\"\"\"abc\"\"\"", true, 0, 2)]
        [InlineData("\"abc\"", "@\"\"\"abc\"\"\"", true, 1, 4)]
        [InlineData("\"abc\"", "@\"\"\"abc\"\"\"", true, 2, 5)]
        [InlineData("\"abc\"", "@\"\"\"abc\"\"\"", true, 3, 6)]
        [InlineData("\"abc\"", "@\"\"\"abc\"\"\"", true, 4, 7)]
        [InlineData("\"abc\"d", "@\"\"\"abc\"\"d\"", true, 5, 9)]
        public void can_map_locations(string valueText, string syntaxText, bool isVerbatim, int givenValueIndex, int expectedSyntaxIndex)
        {
            var locator = new StringLiteralSyntaxCharValueLocator(valueText, syntaxText, isVerbatim);

            var actual = locator.ConvertValueToSyntaxIndex(givenValueIndex);

            actual.Should().Be(expectedSyntaxIndex);
        }
    }
}
