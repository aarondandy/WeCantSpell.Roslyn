using System;

namespace WeCantSpell
{
    public class StringLiteralSyntaxCharValueLocator
    {
        public StringLiteralSyntaxCharValueLocator(
            string valueText,
            string syntaxText,
            bool isVerbatim)
        {
            ValueText = valueText;
            SyntaxText = syntaxText;
            IsVerbatim = isVerbatim;
        }

        public string ValueText { get; }

        public string SyntaxText { get; }

        public bool IsVerbatim { get; }

        public int ConvertValueToSyntaxIndex(int valueIndex)
        {
            if (valueIndex < 0 || valueIndex >= ValueText.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(valueIndex));
            }

            var valueCursor = 0;
            int syntaxCursor;
            if (
                !IsVerbatim
                && SyntaxText.Length != 0
                && SyntaxText[0] == '"'
                && (ValueText.Length == 0 || ValueText[0] != '"')
            )
            {
                syntaxCursor = 1;
            }
            else if (
                IsVerbatim
                && SyntaxText.Length > 1
                && SyntaxText.StartsWith("@\"")
                && !ValueText.StartsWith("@\"")
            )
            {
                syntaxCursor = 2;
            }
            else
            {
                syntaxCursor = 0;
            }

            for (; valueCursor < valueIndex; valueCursor++)
            {
                var valueChar = ValueText[valueCursor];
                var syntaxChar = SyntaxText[syntaxCursor];

                if (IsVerbatim && syntaxChar == '"')
                {
                    syntaxCursor += 2;
                }
                else if (valueChar == syntaxChar)
                {
                    syntaxCursor++;
                }
                else if (!IsVerbatim && syntaxChar == '\\')
                {
                    syntaxCursor++;
                    syntaxChar = SyntaxText[syntaxCursor];

                    if (syntaxChar == 'u')
                    {
                        syntaxCursor += 5; // skip the manditory 4 chars
                    }
                    else if (syntaxChar == 'U')
                    {
                        syntaxCursor++;
                        var hexChars = SyntaxText.Substring(syntaxCursor, Math.Min(SyntaxText.Length - syntaxCursor, 8));
                        syntaxCursor += hexChars.Length;
                    }
                    else if (syntaxChar == 'x')
                    {
                        syntaxCursor++;
                        for (var digitsRead = 0; digitsRead < 4; digitsRead++)
                        {
                            if (!IsHex(SyntaxText[syntaxCursor]))
                            {
                                break;
                            }

                            syntaxCursor++;
                        }
                    }
                    else
                    {
                        syntaxCursor++;
                    }
                }
                else
                {
                    syntaxCursor++;
                }
            }

            return syntaxCursor;
        }

        private static bool IsHex(char c) =>
            (c >= '0' && c <= '9')
            || (c >= 'a' && c <= 'f')
            || (c >= 'A' && c <= 'F');
    }
}
