using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace WeCantSpell.Tests
{
    public class TextLiteralParserTests
    {
        public static IEnumerable<object[]> standard_sentences_extracts_words_args()
        {
            yield return new object[] { "The", 0 };
            yield return new object[] { "quick", 4 };
            yield return new object[] { "brown", 10 };
            yield return new object[] { "fox", 16 };
            yield return new object[] { "jumps", 20 };
            yield return new object[] { "over", 26 };
            yield return new object[] { "the", 31 };
            yield return new object[] { "lazy", 35 };
            yield return new object[] { "dog", 40 };
        }

        [Theory, MemberData(nameof(standard_sentences_extracts_words_args))]
        public void standard_sentences_extracts_words(string expectedText, int offset)
        {
            var sentenceText = "The quick brown fox jumps over the lazy dog.";
            var parser = new TextLiteralParser();

            var results = parser.SplitWordParts(sentenceText);

            var part = results.Should().ContainSingle(x => x.Text == expectedText).Subject;
            part.Start.Should().Be(offset);
            part.IsWord.Should().BeTrue();
        }

        [Fact]
        public void hyphenated_word_is_a_whole_word()
        {
            var sentenceText = "The quick-brown fox jumps-over the lazy dog.";
            var parser = new TextLiteralParser();

            var results = parser.SplitWordParts(sentenceText);

            var firstResult = results.Should().ContainSingle(x => x.Text == "quick-brown").Subject;
            firstResult.Start.Should().Be(4);
            firstResult.IsWord.Should().BeTrue();

            var secondResult = results.Should().ContainSingle(x => x.Text == "jumps-over").Subject;
            secondResult.Start.Should().Be(20);
            secondResult.IsWord.Should().BeTrue();
        }

    }
}
