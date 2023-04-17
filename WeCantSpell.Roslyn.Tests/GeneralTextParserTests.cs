using System.Linq;
using FluentAssertions;
using Xunit;

namespace WeCantSpell.Roslyn.Tests
{
    public class GeneralTextParserTests
    {
        public static object[][] standard_sentences_extracts_words_args() => new[]
        {
            new object[] { "The", 0 },
            new object[] { "quick", 4 },
            new object[] { "brown", 10 },
            new object[] { "fox", 16 },
            new object[] { "jumps", 20 },
            new object[] { "over", 26 },
            new object[] { "the", 31 },
            new object[] { "lazy", 35 },
            new object[] { "dog", 40 }
        };

        [Theory, MemberData(nameof(standard_sentences_extracts_words_args))]
        public void standard_sentences_extracts_words(string expectedText, int offset)
        {
            var sentenceText = "The quick brown fox jumps over the lazy dog.";

            var results = GeneralTextParser.SplitWordParts(sentenceText);

            var part = results.Should().ContainSingle(x => x.Text == expectedText).Subject;
            part.Start.Should().Be(offset);
            part.IsWord.Should().BeTrue();
        }

        [Fact]
        public void hyphenated_word_is_a_whole_word()
        {
            var sentenceText = "The quick-brown fox jumps-over the lazy dog.";

            var results = GeneralTextParser.SplitWordParts(sentenceText);

            var firstResult = results.Should().ContainSingle(x => x.Text == "quick-brown").Subject;
            firstResult.Start.Should().Be(4);
            firstResult.IsWord.Should().BeTrue();

            var secondResult = results.Should().ContainSingle(x => x.Text == "jumps-over").Subject;
            secondResult.Start.Should().Be(20);
            secondResult.IsWord.Should().BeTrue();
        }

        [Fact]
        public void case_word_are_split_correctly()
        {
            var sentenceText = "The quickBrown fox jumps-over the lazy dog.";

            var results = GeneralTextParser.SplitWordParts(sentenceText);

            var firstResult = results.Should().ContainSingle(x => x.Text == "quick").Subject;
            firstResult.Start.Should().Be(4);
            firstResult.IsWord.Should().BeTrue();

            var secondResult = results.Should().ContainSingle(x => x.Text == "Brown").Subject;
            secondResult.Start.Should().Be(9);
            secondResult.IsWord.Should().BeTrue();
        }

        [Fact]
        public void english_contractions_are_split_correctly()
        {
            var given = "you'dn't've a’n't I'da It’s";
            var expected = given.Split(' ');

            var actual = GeneralTextParser.SplitWordParts(given);
            var actualWords = actual.Where(p => p.IsWord).Select(p => p.Text);

            actualWords.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void empty_string_has_no_parts()
        {
            var text = string.Empty;

            var results = GeneralTextParser.SplitWordParts(text);

            results.Should().BeEmpty();
        }

        [Fact]
        public void single_char_text_has_single_part()
        {
            var text = "a";

            var results = GeneralTextParser.SplitWordParts(text);

            results.Should().ContainSingle()
                .Subject.Text.Should().Be(text);
        }
    }
}
