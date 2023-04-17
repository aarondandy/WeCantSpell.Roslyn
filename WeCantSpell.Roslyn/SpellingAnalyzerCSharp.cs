using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace WeCantSpell.Roslyn
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class SpellingAnalyzerCSharp : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor s_spellingIdentifierDiagnosticDescriptor = new(
            "SP3110",
            "Identifier Spelling",
            "Identifier spelling mistake: {0}",
            "Naming",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Identifier name may contain a spelling mistake.");

        private static readonly DiagnosticDescriptor s_spellingLiteralDiagnosticDescriptor = new(
            "SP3111",
            "Text Literal Spelling",
            "Text literal spelling mistake: {0}",
            "Spelling",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Text literal may contain a spelling mistake.");

        private static readonly DiagnosticDescriptor s_spellingCommentDiagnosticDescriptor = new(
            "SP3112",
            "Comment Spelling",
            "Comment spelling mistake: {0}",
            "Spelling",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Comment may contain a spelling mistake.");

        private static readonly DiagnosticDescriptor s_spellingDocumentationDiagnosticDescriptor = new(
            "SP3113",
            "Documentation Spelling",
            "Documentation spelling mistake: {0}",
            "Spelling",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Documentation may contain a spelling mistake.");

        private static readonly ImmutableArray<DiagnosticDescriptor> s_supportedDiagnosticsArray = ImmutableArray.Create(
            s_spellingIdentifierDiagnosticDescriptor,
            s_spellingLiteralDiagnosticDescriptor,
            s_spellingCommentDiagnosticDescriptor,
            s_spellingDocumentationDiagnosticDescriptor);

        public SpellingAnalyzerCSharp()
            : this(new EmbeddedSpellChecker(new [] {"en-US", "ru-RU"} )) { }

        public SpellingAnalyzerCSharp(ISpellChecker spellChecker) => SpellChecker = spellChecker;

        private ISpellChecker SpellChecker { get; }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => s_supportedDiagnosticsArray;

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxTreeAction(HandleSyntaxTree);
        }

        private void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetRoot(context.CancellationToken);
            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            var walker = new SpellCheckCSharpWalker(SpellChecker, ReportMistakeAsDiagnostic);
            walker.Visit(root);

            void ReportMistakeAsDiagnostic(SpellingMistake mistake) =>
                ReportDiagnostic(mistake, context);
        }

        private static void ReportDiagnostic(SpellingMistake mistake, SyntaxTreeAnalysisContext context)
        {
            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            DiagnosticDescriptor descriptor = SelectDescriptor(mistake.Kind);
            var diagnostic = Diagnostic.Create(descriptor, mistake.Location, mistake.Text);
            context.ReportDiagnostic(diagnostic);
        }

        private static DiagnosticDescriptor SelectDescriptor(SpellingMistakeKind kind)
        {
            return kind switch
            {
                SpellingMistakeKind.Identifier => s_spellingIdentifierDiagnosticDescriptor,
                SpellingMistakeKind.Literal => s_spellingLiteralDiagnosticDescriptor,
                SpellingMistakeKind.Comment => s_spellingCommentDiagnosticDescriptor,
                SpellingMistakeKind.Documentation => s_spellingDocumentationDiagnosticDescriptor,
                _ => throw new NotSupportedException()
            };
        }

        /// <summary>
        /// Load .dll dependencies in memory as per https://stackoverflow.com/a/67074009
        /// </summary>
        static SpellingAnalyzerCSharp()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (_, args) =>
            {
                AssemblyName name = new(args.Name);
                Assembly? loadedAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().FullName == name.FullName);
                if (loadedAssembly != null)
                {
                    return loadedAssembly;
                }

                var resourceName = $"{typeof(SpellingAnalyzerCSharp).Namespace}.{name.Name}.dll";

                using Stream? resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
                if (resourceStream == null)
                {
                    return null;
                }

                using var memoryStream = new MemoryStream();
                resourceStream.CopyTo(memoryStream);

                return Assembly.Load(memoryStream.ToArray());
            };
        }
    }
}
