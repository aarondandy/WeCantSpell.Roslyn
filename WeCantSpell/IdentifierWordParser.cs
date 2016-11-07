using System;
using System.Collections.Generic;

namespace WeCantSpell
{
    public class IdentifierWordParser
    {
        public IEnumerable<WordPart> SplitWordParts(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (text.Length == 0)
            {
                return Array.Empty<WordPart>();
            }

            return SplitWordPartsGenerator(text);
        }

        private IEnumerable<WordPart> SplitWordPartsGenerator(string text)
        {
            var partStartIndex = 0;
            var prevType = ClassifyLetterType(text[0]);
            var currType = prevType;

            var searchIndex = 0;
            var nextIndex = 1;
            for(; searchIndex < text.Length; searchIndex = nextIndex++)
            {
                var nextType = nextIndex < text.Length ? ClassifyLetterType(text[nextIndex]) : LetterType.NonWord;

                if (
                    currType == LetterType.LetterUpper && nextType == LetterType.LetterNormal
                    ||
                    (prevType != currType && (prevType != LetterType.LetterUpper || currType != LetterType.LetterNormal))
                )
                {
                    if (searchIndex > partStartIndex)
                    {
                        yield return new WordPart(text.Substring(partStartIndex, searchIndex - partStartIndex), partStartIndex, currType != LetterType.NonWord);
                    }

                    partStartIndex = searchIndex;
                }

                prevType = currType;
                currType = nextType;
            }

            if (partStartIndex < text.Length)
            {
                yield return new WordPart(text.Substring(partStartIndex, text.Length - partStartIndex), partStartIndex, prevType != LetterType.NonWord);
            }
        }

        private static LetterType ClassifyLetterType(char c) =>
            char.IsLetter(c)
                ? (char.IsUpper(c) ? LetterType.LetterUpper : LetterType.LetterNormal)
                : LetterType.NonWord;

        private enum LetterType : byte
        {
            NonWord = 0,
            LetterNormal = 1,
            LetterUpper = 3
        }
    }

    public struct WordPart : IEquatable<WordPart>
    {
        public WordPart(string text, int start, bool isWord)
        {
            Text = text;
            Start = start;
            IsWord = isWord;
        }

        public string Text { get; }

        public int Start { get; }

        public bool IsWord { get; }

        public int Length => Text.Length;

        public int End => Start + Text.Length;

        public bool Equals(WordPart other) =>
            Text == other.Text
            && Start == other.Start
            && IsWord == other.IsWord;

        public override bool Equals(object obj) => obj is WordPart && Equals((WordPart)obj);

        public override int GetHashCode() => unchecked(Text.GetHashCode() ^ Start);
    }
}
