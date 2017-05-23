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

                syntaxCursor++;
                if (IsVerbatim && syntaxChar == '"')
                {
                    syntaxChar = SyntaxText[syntaxCursor];
                    if (syntaxChar == '"')
                    {
                        syntaxCursor++;
                    }
                }
                else if (valueChar == syntaxChar)
                {
                    continue;
                }
                else if (!IsVerbatim && syntaxChar == '\\')
                {
                    ReadEscape(ref syntaxCursor);
                }
            }

            return syntaxCursor;
        }

        private void ReadEscape(ref int syntaxCursor)
        {
            var syntaxChar = SyntaxText[syntaxCursor];

            syntaxCursor++;
            if (syntaxChar == 'u')
            {
                syntaxCursor += Math.Min(SyntaxText.Length - syntaxCursor, 4);
            }
            else if (syntaxChar == 'U')
            {
                syntaxCursor += Math.Min(SyntaxText.Length - syntaxCursor, 8);
            }
            else if (syntaxChar == 'x')
            {
                ReadHexValues(ref syntaxCursor);
            }
        }

        private void ReadHexValues(ref int syntaxCursor)
        {
            for (var digitsRead = 0; digitsRead < 4; digitsRead++)
            {
                if (!IsHex(SyntaxText[syntaxCursor]))
                {
                    break;
                }

                syntaxCursor++;
            }
        }

        private static bool IsHex(char c) =>
            (c >= '0' && c <= '9')
            || (c >= 'a' && c <= 'f')
            || (c >= 'A' && c <= 'F');
    }
}
