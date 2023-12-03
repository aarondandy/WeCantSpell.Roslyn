using System;

namespace WeCantSpell.Roslyn
{
    public sealed class StringLiteralSyntaxCharValueLocator
    {
        public StringLiteralSyntaxCharValueLocator(string valueText, string syntaxText, bool isVerbatim)
        {
            ValueText = valueText;
            SyntaxText = syntaxText;
            IsVerbatim = isVerbatim;
        }

        private string ValueText { get; }

        private string SyntaxText { get; }

        private bool IsVerbatim { get; }

        public int ConvertValueToSyntaxIndex(int valueIndex)
        {
            if (valueIndex < 0 || valueIndex >= ValueText.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(valueIndex));
            }

            var valueCursor = 0;
            int syntaxCursor = IsVerbatim switch
            {
                false
                    when SyntaxText.Length != 0
                        && SyntaxText[0] == '"'
                        && (ValueText.Length == 0 || ValueText[0] != '"')
                    => 1,
                true when SyntaxText.Length > 1 && SyntaxText.StartsWith("@\"") && !ValueText.StartsWith("@\"") => 2,
                _ => 0
            };

            for (; valueCursor < valueIndex; valueCursor++)
            {
                char valueChar = ValueText[valueCursor];
                char syntaxChar = SyntaxText[syntaxCursor];

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
                    // do nothing
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
            char syntaxChar = SyntaxText[syntaxCursor];

            syntaxCursor++;
            switch (syntaxChar)
            {
                case 'u':
                    syntaxCursor += Math.Min(SyntaxText.Length - syntaxCursor, 4);
                    break;
                case 'U':
                    syntaxCursor += Math.Min(SyntaxText.Length - syntaxCursor, 8);
                    break;
                case 'x':
                    ReadHexValues(ref syntaxCursor);
                    break;
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

        private static bool IsHex(char c) => c is >= '0' and <= '9' or >= 'a' and <= 'f' or >= 'A' and <= 'F';
    }
}
