using Microsoft.CodeAnalysis.Text;

namespace WeCantSpell.Utilities
{
    internal static class TextSpanExtensions
    {
        public static bool IsEmpty(TextSpan textSpan)
            => textSpan.IsEmpty;
    }
}
