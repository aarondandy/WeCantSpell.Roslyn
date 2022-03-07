using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace WeCantSpell.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class SpellingAnalyzerCSharp : DiagnosticAnalyzer
    {
        static DiagnosticDescriptor SpellingIdentifierDiagnosticDescriptor = new DiagnosticDescriptor(
            "SP3110",
            "Identifier Spelling",
            "Identifier spelling mistake: {0}",
            "Naming",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Identifier name may contain a spelling mistake.");

        static DiagnosticDescriptor SpellingLiteralDiagnosticDescriptor = new DiagnosticDescriptor(
            "SP3111",
            "Text Literal Spelling",
            "Text literal spelling mistake: {0}",
            "Spelling",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Text literal may contain a spelling mistake.");

        static DiagnosticDescriptor SpellingCommentDiagnosticDescriptor = new DiagnosticDescriptor(
            "SP3112",
            "Comment Spelling",
            "Comment spelling mistake: {0}",
            "Spelling",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Comment may contain a spelling mistake.");

        static DiagnosticDescriptor SpellingDocumentationDiagnosticDescriptor = new DiagnosticDescriptor(
            "SP3113",
            "Documentation Spelling",
            "Documentation spelling mistake: {0}",
            "Spelling",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Documentation may contain a spelling mistake.");

        static ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsArray = ImmutableArray.Create(
            SpellingIdentifierDiagnosticDescriptor,
            SpellingLiteralDiagnosticDescriptor,
            SpellingCommentDiagnosticDescriptor,
            SpellingDocumentationDiagnosticDescriptor);

        public SpellingAnalyzerCSharp()
            : this(new EmbeddedSpellChecker("en-US")) { }

        public SpellingAnalyzerCSharp(ISpellChecker spellChecker) => SpellChecker = spellChecker;

        public ISpellChecker SpellChecker { get; }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => SupportedDiagnosticsArray;

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxTreeAction(HandleSyntaxTree);
        }

        void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            var root = context.Tree.GetRoot(context.CancellationToken);
            if (root == null || context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            var walker = new SpellCheckCSharpWalker(SpellChecker, reportMistakeAsDiagnostic);
            walker.Visit(root);

            void reportMistakeAsDiagnostic(SpellingMistake mistake) =>
                ReportDiagnostic(mistake, context);
        }

        static void ReportDiagnostic(SpellingMistake mistake, SyntaxTreeAnalysisContext context)
        {
            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            var descriptor = SelectDescriptor(mistake.Kind);
            var diagnostic = Diagnostic.Create(descriptor, mistake.Location, mistake.Text);
            context.ReportDiagnostic(diagnostic);
        }

        static DiagnosticDescriptor SelectDescriptor(SpellingMistakeKind kind)
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
