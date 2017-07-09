using System;
using System.Collections.Generic;
using WeCantSpell.Roslyn.Utilities;

namespace WeCantSpell.Roslyn
{
    public static class IdentifierWordParser
    {
        public static List<ParsedTextSpan> SplitWordParts(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var results = ListPool<ParsedTextSpan>.Get();
            if (text.Length == 0)
            {
                return results;
            }

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
                    results.Add(new ParsedTextSpan(text.Substring(partStartIndex, searchIndex - partStartIndex), partStartIndex, prevType != LetterType.NonWord));

                    partStartIndex = searchIndex;
                }

                prevType = currType;
                currType = nextType;
            }

            if (partStartIndex < text.Length)
            {
                results.Add(new ParsedTextSpan(text.Substring(partStartIndex, text.Length - partStartIndex), partStartIndex, prevType != LetterType.NonWord));
            }

            return results;
        }

        static LetterType ClassifyLetterType(char c) =>
            char.IsLetter(c)
                ? (char.IsUpper(c) ? LetterType.LetterUpper : LetterType.LetterNormal)
                : LetterType.NonWord;

        enum LetterType : byte
        {
            NonWord = 0,
            LetterNormal = 1,
            LetterUpper = 3
        }
    }
}
