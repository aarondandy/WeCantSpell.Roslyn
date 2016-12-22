using Microsoft.CodeAnalysis.Text;

namespace WeCantSpell
{
    public static class CommentTextExtractor
    {
        public static TextSpan LocateSingleLineCommentText(string commentText)
        {
            var startIndex = 0;

            // skip initial whitespace
            for (; startIndex < commentText.Length; startIndex++)
            {
                if (!IsCSharpWhitespace(commentText[startIndex]))
                {
                    break;
                }
            }

            // skip the initial slashes
            for (; startIndex < commentText.Length; startIndex++)
            {
                if (commentText[startIndex] != '/')
                {
                    break;
                }
            }

            // skip following whitespace
            for (; startIndex < commentText.Length; startIndex++)
            {
                if (!IsCSharpWhitespace(commentText[startIndex]))
                {
                    break;
                }
            }

            var endIndex = commentText.Length - 1;
            for (; endIndex >= startIndex; endIndex--)
            {
                if (!IsCSharpWhitespace(commentText[endIndex]))
                {
                    break;
                }
            }

            return new TextSpan(startIndex, endIndex - startIndex + 1);
        }

        private static bool IsCSharpWhitespace(char c)
        {
            // from spec
            return char.IsWhiteSpace(c)
                || c == '\u0009'
                || c == '\u000b'
                || c == '\u000c';
        }
    }
}
