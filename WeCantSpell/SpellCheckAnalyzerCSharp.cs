using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace WeCantSpell
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SpellCheckAnalyzerCSharp : DiagnosticAnalyzer
    {
        public const string DiagnosticId = nameof(WeCantSpell);

        private static DiagnosticDescriptor DiagnosticDescriptor = new DiagnosticDescriptor(
            DiagnosticId,
            "Title",
            "MessageFormat",
            "Naming",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Description");

        private static ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticArray = ImmutableArray.Create(DiagnosticDescriptor);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => SupportedDiagnosticArray;

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            // TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
            var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

            // Find just those named type symbols with names containing lowercase letters.
            if (namedTypeSymbol.Name.ToCharArray().Any(char.IsLower))
            {
                // For all such symbols, produce a diagnostic.
                var diagnostic = Diagnostic.Create(DiagnosticDescriptor, namedTypeSymbol.Locations[0]);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
