using Microsoft.CodeAnalysis.Text;

namespace WeCantSpell.Roslyn.Utilities
{
    static class TextSpanExtensions
    {
        public static bool IsEmpty(TextSpan textSpan)
            => textSpan.IsEmpty;
    }
}
