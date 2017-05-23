using System;
using System.Collections.Generic;
using System.Linq;

namespace WeCantSpell
{
    public static class IdentifierWordParser
    {
        public static IEnumerable<ParsedTextSpan> SplitWordParts(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (text.Length == 0)
            {
                return Enumerable.Empty<ParsedTextSpan>();
            }

            return SplitWordPartsGenerator(text);
        }

        private static IEnumerable<ParsedTextSpan> SplitWordPartsGenerator(string text)
        {
            var partStartIndex = 0;
            var prevType = ClassifyLetterType(text[0]);
            var currType = text.Length > 1 ? ClassifyLetterType(text[1]) : prevType;

            for (int searchIndex = 1, nextIndex = 2; searchIndex < text.Length; searchIndex = nextIndex++)
            {
                var nextType = nextIndex < text.Length ? ClassifyLetterType(text[nextIndex]) : LetterType.NonWord;

                if (
                    currType == LetterType.LetterUpper && nextType == LetterType.LetterNormal
                    ||
                    (prevType != currType && (prevType != LetterType.LetterUpper || currType != LetterType.LetterNormal))
                )
                {
                    yield return new ParsedTextSpan(text.Substring(partStartIndex, searchIndex - partStartIndex), partStartIndex, prevType != LetterType.NonWord);

                    partStartIndex = searchIndex;
                }

                prevType = currType;
                currType = nextType;
            }

            if (partStartIndex < text.Length)
            {
                yield return new ParsedTextSpan(text.Substring(partStartIndex, text.Length - partStartIndex), partStartIndex, prevType != LetterType.NonWord);
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
}
