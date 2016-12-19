using System;
using System.Collections.Generic;

namespace WeCantSpell
{
    public class GeneralTextParser
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
            var prevCharType = ClassifyCharType(prevChar);

            char currChar;
            CharType currCharType;

            if (text.Length > 1)
            {
                currChar = text[1];
                currCharType = ClassifyCharType(currChar);
            }
            else
            {
                currChar = '\0';
                currCharType = prevCharType;
            }

            var prevEffectiveType = GetEffectiveCharType(prevChar, prevCharType, CharType.Unknown, currCharType);

            for (int searchIndex = 1, nextIndex = 2; searchIndex < text.Length; searchIndex = nextIndex++)
            {
                char nextChar;
                CharType nextCharType;
                if (nextIndex < text.Length)
                {
                    nextChar = text[nextIndex];
                    nextCharType = ClassifyCharType(nextChar);
                }
                else
                {
                    nextChar = '\0';
                    nextCharType = CharType.WhiteSpace;
                }

                var currEffectiveType = GetEffectiveCharType(currChar, currCharType, prevCharType, nextCharType);
                var currentIsWord = currEffectiveType == CharType.Word;
                var previousWasWord = prevEffectiveType == CharType.Word;

                if (currentIsWord != previousWasWord)
                {
                    yield return new ParsedTextSpan(text.Substring(partStartIndex, searchIndex - partStartIndex), partStartIndex, previousWasWord);

                    partStartIndex = searchIndex;
                }

                prevChar = currChar;
                prevCharType = currCharType;
                prevEffectiveType = currEffectiveType;

                currChar = nextChar;
                currCharType = nextCharType;
            }

            if (partStartIndex < text.Length)
            {
                yield return new ParsedTextSpan(text.Substring(partStartIndex, text.Length - partStartIndex), partStartIndex, prevEffectiveType == CharType.Word);
            }
        }

        private static CharType GetEffectiveCharType(char currChar, CharType currCharType, CharType prevCharType, CharType nextCharType)
        {
            if (
                currCharType != CharType.Word
                &&
                prevCharType == CharType.Word
                &&
                nextCharType == CharType.Word
                &&
                IsHyphen(currChar)
            )
            {
                return CharType.Word;
            }

            return currCharType;
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
