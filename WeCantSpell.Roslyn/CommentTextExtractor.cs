using System.Collections.Generic;
using System.Globalization;
using Microsoft.CodeAnalysis.Text;

namespace WeCantSpell.Roslyn
{
    public static class CommentTextExtractor
    {
        private const char CommentSlashChar = '/';
        private const char CommentStarChar = '*';

        public static TextSpan LocateSingleLineCommentText(string commentText)
        {
            var startIndex = 0;
            int endIndex = commentText.Length;

            // skip initial whitespace
            for (; startIndex < endIndex && IsCSharpWhitespace(commentText[startIndex]); startIndex++) ;

            // skip the initial slashes
            for (; startIndex < endIndex && commentText[startIndex] == CommentSlashChar; startIndex++) ;

            // skip following whitespace
            for (; startIndex < endIndex && IsCSharpWhitespace(commentText[startIndex]); startIndex++) ;

            // skip trailing whitespace
            for (; startIndex < endIndex && IsCSharpWhitespace(commentText[endIndex - 1]); endIndex--) ;

            return TextSpan.FromBounds(startIndex, endIndex);
        }

        public static List<TextSpan> LocateMultiLineCommentTextParts(string text)
        {
            List<TextSpan> allLines = LocateLines(text);

            for (var i = 0; i < allLines.Count; i++)
            {
                allLines[i] = TrimMultiLinePartToTextPart(text, allLines[i]);
            }

            allLines.RemoveAll(s => s.IsEmpty);

            return allLines;
        }

        private static TextSpan TrimMultiLinePartToTextPart(string commentText, TextSpan lineSpan)
        {
            int startIndex = lineSpan.Start;
            int endIndex = lineSpan.End;

            // skip initial whitespace
            for (; startIndex < endIndex && IsCSharpWhitespace(commentText[startIndex]); startIndex++) ;

            // skip the slashes
            for (; startIndex < endIndex && commentText[startIndex] == CommentSlashChar; startIndex++) ;

            // skip the stars
            for (; startIndex < endIndex && commentText[startIndex] == CommentStarChar; startIndex++) ;

            // skip following whitespace after //**
            for (; startIndex < endIndex && IsCSharpWhitespace(commentText[startIndex]); startIndex++) ;

            // skip trailing whitespace
            for (; startIndex < endIndex && IsCSharpWhitespace(commentText[endIndex - 1]); endIndex--) ;

            // skip trailing slashes
            for (; startIndex < endIndex && commentText[endIndex - 1] == CommentSlashChar; endIndex--) ;

            // skip trailing stars
            for (; startIndex < endIndex && commentText[endIndex - 1] == CommentStarChar; endIndex--) ;

            // skip trailing whitespace before **//
            for (; startIndex < endIndex && IsCSharpWhitespace(commentText[endIndex - 1]); endIndex--) ;

            return TextSpan.FromBounds(startIndex, endIndex);
        }

        private static List<TextSpan> LocateLines(string text)
        {
            var startIndex = 0;
            var result = new List<TextSpan>();

            for (var scanIndex = 0; scanIndex < text.Length; scanIndex++)
            {
                if (IsLineBreak(text[scanIndex]))
                {
                    if (startIndex != scanIndex)
                    {
                        result.Add(new TextSpan(startIndex, scanIndex - startIndex));
                    }

                    startIndex = scanIndex + 1;
                }
            }

            if (startIndex < text.Length - 1)
            {
                result.Add(TextSpan.FromBounds(startIndex, text.Length));
            }

            return result;
        }

        private static bool IsCSharpWhitespace(char c)
        {
            switch (c)
            {
                case '\u0009': // NOTE: horizontal tab is called out in the ECMA spec for C#
                case '\u000b': // NOTE: vertical tab is called out in the ECMA spec for C#
                case '\u000c': // NOTE: form feed is called out in the ECMA spec for C#
                case ' ': // NOTE: space is part of the Zs character class
                case '\u00a0': // NOTE: no-break space is part of the Zs character class
                    return true;
                default:
                    // NOTE: all values that are in the Zs class and in the ASCII range are covered on explicit cases
                    return c > 0xff
                        // NOTE: the Zs class is considered whitespace in C#
                        && CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.SpaceSeparator;
            }
        }

        private static bool IsLineBreak(char c)
        {
            switch (c)
            {
                case '\r': // NOTE: CR or U+000d is part of the ECMA spec
                case '\n': // NOTE: LF or U+000a is part of the ECMA spec
                case '\u2028': // NOTE: line separator or U+2028 is a valid line terminator in the ECMA spec
                case '\u2029': // NOTE: paragraph separator or U+2029 is a valid line terminator in the ECMA spec
                case '\u2085': // NOTE: next line or U+2085 is a valid line terminator in the ECMA spec
                    return true;
                default:
                    return false;
            }
        }
    }
}
