using System;
using System.Collections.Generic;

namespace WeCantSpell
{
    public class TextLiteralParser
    {
        public IEnumerable<ParsedTextSpan> SplitWordParts(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (text.Length == 0)
            {
                return Array.Empty<ParsedTextSpan>();
            }

            return SplitWordPartsGenerator(text);
        }

        private IEnumerable<ParsedTextSpan> SplitWordPartsGenerator(string text)
        {
            var partStartIndex = 0;
            var prevChar = text[0];
            var prevType = ClassifyCharType(prevChar);

            char currChar;
            CharType currType;

            if (text.Length > 1)
            {
                currChar = text[1];
                currType = ClassifyCharType(currChar);
            }
            else
            {
                currChar = '\0';
                currType = prevType;
            }

            var previousWasWord = prevType == CharType.Word;

            for (int searchIndex = 1, nextIndex = 2; searchIndex < text.Length; searchIndex = nextIndex++)
            {
                char nextChar;
                CharType nextType;
                if (nextIndex < text.Length)
                {
                    nextChar = text[nextIndex];
                    nextType = ClassifyCharType(nextChar);
                }
                else
                {
                    nextChar = '\0';
                    nextType = CharType.WhiteSpace;
                }

                var currentIsWord = currType == CharType.Word;

                if (
                    !currentIsWord
                    && prevType == CharType.Word
                    && nextType == CharType.Word
                    && IsHyphen(currChar))
                {
                    currentIsWord = true;
                }

                if (currentIsWord != previousWasWord)
                {
                    yield return new ParsedTextSpan(text.Substring(partStartIndex, searchIndex - partStartIndex), partStartIndex, previousWasWord);

                    partStartIndex = searchIndex;
                }

                previousWasWord = currentIsWord;
                prevType = currType;
                prevChar = currChar;

                currType = nextType;
                currChar = nextChar;
            }

            if (partStartIndex < text.Length)
            {
                yield return new ParsedTextSpan(text.Substring(partStartIndex, text.Length - partStartIndex), partStartIndex, previousWasWord);
            }
        }

        private static CharType ClassifyCharType(char current)
        {
            if (char.IsLetterOrDigit(current))
            {
                return CharType.Word;
            }
            if (char.IsWhiteSpace(current))
            {
                return CharType.WhiteSpace;
            }
            if (char.IsPunctuation(current))
            {
                return CharType.Punctuation;
            }

            return CharType.Unknown;
        }

        private static bool IsHyphen(char c)
        {
            return c == '-'
                || c == '\u2010'
                || c == '\u2212'
                || c == '\u2014'
                || c == '\u2013';
        }

        private enum CharType
        {
            Unknown = 0,
            Word = 1,
            WhiteSpace = 2,
            Punctuation = 4
        }
    }
}
