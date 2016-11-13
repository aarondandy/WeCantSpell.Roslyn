using Microsoft.CodeAnalysis;

namespace WeCantSpell.Tests.Utilities
{
    public static class AssertionExtensions
    {
        public static DiagnosticAssertions Should(this Diagnostic diagnostic) => new DiagnosticAssertions(diagnostic);
    }
}
