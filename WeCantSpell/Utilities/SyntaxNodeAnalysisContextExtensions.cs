using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace WeCantSpell.Utilities
{
    internal static class SyntaxNodeAnalysisContextExtensions
    {
        public static void ReportDiagnostics(this SyntaxNodeAnalysisContext context, IEnumerable<Diagnostic> diagnostics)
        {
            foreach (var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        public static void ReportDiagnostics(this SyntaxTreeAnalysisContext context, IEnumerable<Diagnostic> diagnostics)
        {
            foreach(var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
