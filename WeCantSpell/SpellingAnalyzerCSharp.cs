using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using WeCantSpell.Utilities;
using System.Collections.Generic;

namespace WeCantSpell
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class SpellingAnalyzerCSharp : DiagnosticAnalyzer
    {
        private static DiagnosticDescriptor SpellingIdentifierDiagnosticDescriptor = new DiagnosticDescriptor(
            "SP3110",
            "Identifier Spelling",
            "Identifier spelling mistake: {0}",
            "Naming",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Identifier name may contain a spelling mistake.");

        private static DiagnosticDescriptor SpellingLiteralDiagnosticDescriptor = new DiagnosticDescriptor(
            "SP3111",
            "Text Literal Spelling",
            "Text literal spelling mistake: {0}",
            "Spelling",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Text literal may contain a spelling mistake.");

        private static DiagnosticDescriptor SpellingCommentDiagnosticDescriptor = new DiagnosticDescriptor(
            "SP3112",
            "Comment Spelling",
            "Comment spelling mistake: {0}",
            "Spelling",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Comment may contain a spelling mistake.");

        private static DiagnosticDescriptor SpellingDocumentationDiagnosticDescriptor = new DiagnosticDescriptor(
            "SP3113",
            "Documentation Spelling",
            "Documentation spelling mistake: {0}",
            "Spelling",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Documentation may contain a spelling mistake.");

        private static ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsArray = ImmutableArray.Create(
            SpellingIdentifierDiagnosticDescriptor,
            SpellingLiteralDiagnosticDescriptor,
            SpellingCommentDiagnosticDescriptor,
            SpellingDocumentationDiagnosticDescriptor);

        public SpellingAnalyzerCSharp()
            : this(new DebugTestingSpellChecker()) { }

        public SpellingAnalyzerCSharp(ISpellChecker spellChecker) => SpellChecker = spellChecker;

        public ISpellChecker SpellChecker { get; }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => SupportedDiagnosticsArray;

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeAction(HandleSyntaxTree);
        }

        private void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            var root = context.Tree.GetRoot(context.CancellationToken);
            if (root == null)
            {
                return;
            }

            var walker = new SpellCheckCSharpWalker(SpellChecker);
            walker.Visit(root);

            if (!context.CancellationToken.IsCancellationRequested)
            {
                ReportDiagnostics(walker.Mistakes, context);
            }
        }

        private static void ReportDiagnostics(List<SpellingMistake> mistakes, SyntaxTreeAnalysisContext context)
        {
            foreach(var mistake in mistakes)
            {
                var diagnostic = ConverToDiagnostic(mistake);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static Diagnostic ConverToDiagnostic(SpellingMistake mistake) =>
            Diagnostic.Create(SelectDescriptor(mistake.Kind), mistake.Location, mistake.Text);

        private static DiagnosticDescriptor SelectDescriptor(SpellingMistakeKind kind)
        {
            switch (kind)
            {
                case SpellingMistakeKind.Identifier: return SpellingIdentifierDiagnosticDescriptor;
                case SpellingMistakeKind.Literal: return SpellingLiteralDiagnosticDescriptor;
                case SpellingMistakeKind.Comment: return SpellingCommentDiagnosticDescriptor;
                case SpellingMistakeKind.Documentation: return SpellingDocumentationDiagnosticDescriptor;
                default: throw new NotSupportedException();
            }
        }
    }
}
