using System;
using System.Collections.Generic;
using System.Linq;

namespace WeCantSpell.Roslyn
{
    public static class GeneralTextParser
    {
        public static List<ParsedTextSpan> SplitWordParts(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var results = new List<ParsedTextSpan>();
            if (text.Length == 0)
            {
                return results;
            }

            var partStartIndex = 0;
            char prevChar = text[0];
            CharType prevCharType = ClassifyCharType(prevChar);

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

            CharType prevEffectiveType = GetEffectiveCharType(prevChar, prevCharType, CharType.Unknown, currCharType);

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

                CharType currEffectiveType = GetEffectiveCharType(currChar, currCharType, prevCharType, nextCharType);
                bool currentIsWord = currEffectiveType == CharType.Word;
                bool previousWasWord = prevEffectiveType == CharType.Word;

                if (currentIsWord != previousWasWord)
                {
                    AddParsedFragmentToResults(text, results, partStartIndex, searchIndex, previousWasWord);

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
                AddParsedFragmentToResults(
                    text,
                    results,
                    partStartIndex,
                    text.Length,
                    prevEffectiveType == CharType.Word
                );
            }

            return results;
        }

        private static void AddParsedFragmentToResults(
            string text,
            ICollection<ParsedTextSpan> results,
            int startIndex,
            int endIndex,
            bool isWord
        )
        {
            var fragment = text.Substring(startIndex, endIndex - startIndex);
            if (!isWord)
            {
                results.Add(new ParsedTextSpan(fragment, startIndex, false));
                return;
            }

            // Detect camelCase or PascalCase or ident123
            var upperCount = fragment.Count(char.IsUpper);
            var numberCount = fragment.Count(char.IsDigit);
            var mayBeIdentifier =
                upperCount > 1
                || numberCount > 0 && !char.IsDigit(fragment.First())
                || !char.IsUpper(fragment.First()) && upperCount == 1;
            if (mayBeIdentifier)
            {
                var wordParts = IdentifierWordParser.SplitWordParts(fragment);
                foreach (var part in wordParts)
                {
                    results.Add(part);
                }
                return;
            }
            results.Add(new ParsedTextSpan(fragment, startIndex, true));
        }

        private static CharType GetEffectiveCharType(
            char currChar,
            CharType currCharType,
            CharType prevCharType,
            CharType nextCharType
        ) =>
            currCharType != CharType.Word
            && prevCharType == CharType.Word
            && nextCharType == CharType.Word
            && IsWordJoinChar(currChar)
                ? CharType.Word
                : currCharType;

        private static CharType ClassifyCharType(char current)
        {
            return char.IsLetterOrDigit(current)
                ? CharType.Word
                : char.IsWhiteSpace(current)
                    ? CharType.WhiteSpace
                    : char.IsPunctuation(current)
                        ? CharType.Punctuation
                        : CharType.Unknown;
        }

        private static bool IsWordJoinChar(char c) => IsHyphen(c) || IsApostrophe(c);

        private static bool IsHyphen(char c) => c is '-' or '\u2010' or '\u2212' or '\u2014' or '\u2013';

        private static bool IsApostrophe(char c) => c is '\'' or '’' or 'ʼ' or '＇'; // full width

        private enum CharType : byte
        {
            Unknown = 0,
            Word = 1,
            WhiteSpace = 2,
            Punctuation = 4
        }
    }
}
