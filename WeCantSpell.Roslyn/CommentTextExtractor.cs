using System.Collections.Generic;
using Microsoft.CodeAnalysis.Text;
using WeCantSpell.Roslyn.Utilities;

namespace WeCantSpell.Roslyn
{
    public static class CommentTextExtractor
    {
        const char CommentSlashChar = '/';
        const char CommentStarChar = '*';

        public static TextSpan LocateSingleLineCommentText(string commentText)
        {
            var startIndex = 0;
            var endIndex = commentText.Length - 1;

            // skip initial whitespace
            for (; startIndex <= endIndex && IsCSharpWhitespace(commentText[startIndex]); startIndex++) ;

            // skip the initial slashes
            for (; startIndex <= endIndex && commentText[startIndex] == CommentSlashChar; startIndex++) ;

            // skip following whitespace
            for (; startIndex <= endIndex && IsCSharpWhitespace(commentText[startIndex]); startIndex++) ;

            // skip trailing whitespace
            for (; endIndex >= startIndex && IsCSharpWhitespace(commentText[endIndex]); endIndex--) ;

            return TextSpan.FromBounds(startIndex, endIndex + 1);
        }

        public static List<TextSpan> LocateMultiLineCommentTextParts(string text)
        {
            var allLines = LocateLines(text);

            for (var i = 0; i < allLines.Count; i++)
            {
                var lineSpan = allLines[i];
                var textSpan = TrimMultiLinePartToTextPart(text, lineSpan);
                allLines[i] = textSpan;
            }

            allLines.RemoveAll(TextSpanExtensions.IsEmpty);

            return allLines;
        }

        static TextSpan TrimMultiLinePartToTextPart(string commentText, TextSpan lineSpan)
        {
            var startIndex = lineSpan.Start;
            var endIndex = lineSpan.End - 1;

            // skip initial whitespace
            for (; startIndex <= endIndex && IsCSharpWhitespace(commentText[startIndex]); startIndex++) ;

            // skip the slashes
            for (; startIndex <= endIndex && commentText[startIndex] == CommentSlashChar; startIndex++) ;

            // skip the stars
            for (; startIndex <= endIndex && commentText[startIndex] == CommentStarChar; startIndex++) ;

            // skip following whitespace after //**
            for (; startIndex <= endIndex && IsCSharpWhitespace(commentText[startIndex]); startIndex++) ;

            // skip trailing whitespace
            for (; endIndex >= startIndex && IsCSharpWhitespace(commentText[endIndex]); endIndex--) ;

            // skip trailing slashes
            for (; endIndex >= startIndex && commentText[endIndex] == CommentSlashChar; endIndex--) ;

            // skip trailing stars
            for (; endIndex >= startIndex && commentText[endIndex] == CommentStarChar; endIndex--) ;

            // skip trailing whitespace before **//
            for (; endIndex >= startIndex && IsCSharpWhitespace(commentText[endIndex]); endIndex--) ;

            return TextSpan.FromBounds(startIndex, endIndex + 1);
        }

        static List<TextSpan> LocateLines(string text)
        {
            var startIndex = 0;
            var result = ListPool<TextSpan>.Get();

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

        static bool IsCSharpWhitespace(char c) =>
            // NOTE: from spec
            c == ' '
            || char.IsWhiteSpace(c)
            || c == '\u0009'
            || c == '\u000b'
            || c == '\u000c';

        static bool IsLineBreak(char c) => c == '\r' || c == '\n';
    }
}
