using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Text;

namespace WeCantSpell
{
    public static class CommentTextExtractor
    {
        public static TextSpan LocateSingleLineCommentText(string commentText)
        {
            var startIndex = 0;

            // skip initial whitespace
            for (; startIndex < commentText.Length && IsCSharpWhitespace(commentText[startIndex]); startIndex++) ;

            // skip the initial slashes
            for (; startIndex < commentText.Length && commentText[startIndex] == '/'; startIndex++) ;

            // skip following whitespace
            for (; startIndex < commentText.Length && IsCSharpWhitespace(commentText[startIndex]); startIndex++) ;

            var endIndex = commentText.Length - 1;

            // skip trailing whitespace
            for (; endIndex >= startIndex && IsCSharpWhitespace(commentText[endIndex]); endIndex--) ;

            return TextSpan.FromBounds(startIndex, endIndex + 1);
        }

        public static IEnumerable<TextSpan> LocateMultiLineCommentTextParts(string text)
        {
            foreach (var lineSpan in LocateLines(text))
            {
                var textSpan = TrimMultiLinePartToTextPart(text, lineSpan);
                if (textSpan.Length != 0)
                {
                    yield return textSpan;
                }
            }
        }

        private static TextSpan TrimMultiLinePartToTextPart(string commentText, TextSpan lineSpan)
        {
            var startIndex = lineSpan.Start;
            var endIndex = lineSpan.End - 1;

            // skip initial whitespace
            for (; startIndex < endIndex && IsCSharpWhitespace(commentText[startIndex]); startIndex++) ;

            // skip the slashes
            for (; startIndex < endIndex && commentText[startIndex] == '/'; startIndex++) ;

            // skip the stars
            for (; startIndex < endIndex && commentText[startIndex] == '*'; startIndex++) ;

            // skip following whitespace after //**
            for (; startIndex < endIndex && IsCSharpWhitespace(commentText[startIndex]); startIndex++) ;

            // skip trailing whitespace
            for (; endIndex >= startIndex && IsCSharpWhitespace(commentText[endIndex]); endIndex--) ;

            // skip trailing slashes
            for (; endIndex >= startIndex && commentText[endIndex] == '/'; endIndex--) ;

            // skip trailing stars
            for (; endIndex >= startIndex && commentText[endIndex] == '*'; endIndex--) ;

            // skip trailing whitespace before **//
            for (; endIndex >= startIndex && IsCSharpWhitespace(commentText[endIndex]); endIndex--) ;

            return TextSpan.FromBounds(startIndex, endIndex + 1);

        }

        private static IEnumerable<TextSpan> LocateLines(string text)
        {
            var startIndex = 0;

            for (var scanIndex = 0; scanIndex < text.Length; scanIndex++)
            {
                if (IsLineBreak(text[scanIndex]))
                {
                    if (startIndex != scanIndex)
                    {
                        yield return new TextSpan(startIndex, scanIndex - startIndex);
                    }

                    startIndex = scanIndex + 1;
                }
            }

            if (startIndex < text.Length - 1)
            {
                yield return TextSpan.FromBounds(startIndex, text.Length);
            }
        }

        private static bool IsCSharpWhitespace(char c) =>
            // from spec
            c == ' '
            || char.IsWhiteSpace(c)
            || c == '\u0009'
            || c == '\u000b'
            || c == '\u000c';

        private static bool IsLineBreak(char c) => c == '\r' || c == '\n';
    }
}
