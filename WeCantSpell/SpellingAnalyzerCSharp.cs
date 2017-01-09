using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using WeCantSpell.Utilities;

namespace WeCantSpell
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class SpellingAnalyzerCSharp : DiagnosticAnalyzer
    {
        public SpellingAnalyzerCSharp()
        {
        }

        public SpellingAnalyzerCSharp(ISpellChecker spellChecker)
        {
            SpellChecker = spellChecker;
        }

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

        private static DiagnosticDescriptor CommentDiagnosticDescriptor = new DiagnosticDescriptor(
            "SP3112",
            "Comment Spelling",
            "Comment spelling mistake: {0}",
            "Spelling",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Comment may contain a spelling mistake.");

        private static DiagnosticDescriptor DocumentationDiagnosticDescriptor = new DiagnosticDescriptor(
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
            CommentDiagnosticDescriptor,
            DocumentationDiagnosticDescriptor);

        public ISpellChecker SpellChecker { get; private set; }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => SupportedDiagnosticsArray;

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(ClassDeclarationHandler, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(StructDeclarationHandler, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(InterfaceDeclarationHandler, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(FieldDeclarationHandler, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeAction(LocalDeclarationStatementHandler, SyntaxKind.LocalDeclarationStatement);
            context.RegisterSyntaxNodeAction(MethodDeclarationHandler, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(ParameterHandler, SyntaxKind.Parameter);
            context.RegisterSyntaxNodeAction(PropertyDeclarationHandler, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(UsingDirectiveHandler, SyntaxKind.UsingDirective);
            context.RegisterSyntaxNodeAction(UsingStatementHandler, SyntaxKind.UsingStatement);
            context.RegisterSyntaxNodeAction(CatchDeclarationHandler, SyntaxKind.CatchDeclaration);
            context.RegisterSyntaxNodeAction(EnumDeclarationHandler, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeAction(EnumMemberDeclarationHandler, SyntaxKind.EnumMemberDeclaration);
            context.RegisterSyntaxNodeAction(AnonymousObjectMemberDeclaratorHandler, SyntaxKind.AnonymousObjectMemberDeclarator);
            context.RegisterSyntaxNodeAction(ForEachStatementHandler, SyntaxKind.ForEachStatement);
            context.RegisterSyntaxNodeAction(ForStatementHandler, SyntaxKind.ForStatement);
            context.RegisterSyntaxNodeAction(LabeledStatementHandler, SyntaxKind.LabeledStatement);
            context.RegisterSyntaxNodeAction(DelegateDeclarationHandler, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(EventDeclarationHandler, SyntaxKind.EventDeclaration);
            context.RegisterSyntaxNodeAction(EventFieldDeclarationHandler, SyntaxKind.EventFieldDeclaration);
            context.RegisterSyntaxNodeAction(TypeParameterHandler, SyntaxKind.TypeParameter);
            context.RegisterSyntaxNodeAction(FromClauseHandler, SyntaxKind.FromClause);
            context.RegisterSyntaxNodeAction(QueryContinuationHandler, SyntaxKind.QueryContinuation);
            context.RegisterSyntaxNodeAction(LetClauseHandler, SyntaxKind.LetClause);
            context.RegisterSyntaxNodeAction(JoinClauseHandler, SyntaxKind.JoinClause);
            context.RegisterSyntaxNodeAction(JoinIntoClauseHandler, SyntaxKind.JoinIntoClause);

            context.RegisterSyntaxNodeAction(StringLiteralExpressionHandler, SyntaxKind.StringLiteralExpression);
            context.RegisterSyntaxNodeAction(InterpolatedStringTextHandler, SyntaxKind.InterpolatedStringText);

            context.RegisterSyntaxTreeAction(this.HandleSyntaxTree);
        }

        private void AnalyzerNotImplemented(SyntaxNodeAnalysisContext context)
        {
            var filePath = context.Node.SyntaxTree.FilePath;
            var node = context.Node;
        }

        private void ClassDeclarationHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (ClassDeclarationSyntax)context.Node;
            context.ReportDiagnostics(GenerateSpellingDiagnosticsForIdentifier(node.Identifier));
        }

        private void StructDeclarationHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (StructDeclarationSyntax)context.Node;
            context.ReportDiagnostics(GenerateSpellingDiagnosticsForIdentifier(node.Identifier));
        }

        private void InterfaceDeclarationHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (InterfaceDeclarationSyntax)context.Node;
            context.ReportDiagnostics(GenerateSpellingDiagnosticsForInterfaceIdentifier(node.Identifier));
        }

        private void FieldDeclarationHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (FieldDeclarationSyntax)context.Node;
            foreach (var field in node.Declaration.Variables)
            {
                context.ReportDiagnostics(GenerateSpellingDiagnosticsForFieldIdentifier(field.Identifier));
            }
        }

        private void LocalDeclarationStatementHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (LocalDeclarationStatementSyntax)context.Node;
            foreach (var variable in node.Declaration.Variables)
            {
                context.ReportDiagnostics(GenerateSpellingDiagnosticsForIdentifier(variable.Identifier));
            }
        }

        private void MethodDeclarationHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (MethodDeclarationSyntax)context.Node;
            context.ReportDiagnostics(GenerateSpellingDiagnosticsForIdentifier(node.Identifier));
        }

        private void ParameterHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (ParameterSyntax)context.Node;
            context.ReportDiagnostics(GenerateSpellingDiagnosticsForIdentifier(node.Identifier));
        }

        private void PropertyDeclarationHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (PropertyDeclarationSyntax)context.Node;
            context.ReportDiagnostics(GenerateSpellingDiagnosticsForIdentifier(node.Identifier));
        }

        private void UsingDirectiveHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (UsingDirectiveSyntax)context.Node;
            var aliasName = node.Alias?.Name;
            if (aliasName != null)
            {
                context.ReportDiagnostics(GenerateSpellingDiagnosticsForIdentifier(aliasName.Identifier));
            }
        }

        private void UsingStatementHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (UsingStatementSyntax)context.Node;
            foreach (var variableDeclaration in node.Declaration.Variables)
            {
                context.ReportDiagnostics(GenerateSpellingDiagnosticsForIdentifier(variableDeclaration.Identifier));
            }
        }

        private void CatchDeclarationHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (CatchDeclarationSyntax)context.Node;
            if (node.Identifier.Span.Length != 0)
            {
                context.ReportDiagnostics(GenerateSpellingDiagnosticsForIdentifier(node.Identifier));
            }
        }

        private void EnumDeclarationHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (EnumDeclarationSyntax)context.Node;
            context.ReportDiagnostics(GenerateSpellingDiagnosticsForIdentifier(node.Identifier));
        }

        private void EnumMemberDeclarationHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (EnumMemberDeclarationSyntax)context.Node;
            context.ReportDiagnostics(GenerateSpellingDiagnosticsForIdentifier(node.Identifier));
        }

        private void AnonymousObjectMemberDeclaratorHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (AnonymousObjectMemberDeclaratorSyntax)context.Node;
            var name = node.NameEquals?.Name;
            if (name != null)
            {
                context.ReportDiagnostics(GenerateSpellingDiagnosticsForIdentifier(name.Identifier));
            }
        }

        private void ForEachStatementHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (ForEachStatementSyntax)context.Node;
            context.ReportDiagnostics(GenerateSpellingDiagnosticsForIdentifier(node.Identifier));
        }

        private void ForStatementHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (ForStatementSyntax)context.Node;
            foreach (var variableDeclaration in node.Declaration.Variables)
            {
                context.ReportDiagnostics(GenerateSpellingDiagnosticsForIdentifier(variableDeclaration.Identifier));
            }
        }

        private void LabeledStatementHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (LabeledStatementSyntax)context.Node;
            context.ReportDiagnostics(GenerateSpellingDiagnosticsForIdentifier(node.Identifier));
        }

        private void DelegateDeclarationHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (DelegateDeclarationSyntax)context.Node;
            context.ReportDiagnostics(GenerateSpellingDiagnosticsForIdentifier(node.Identifier));
        }

        private void EventDeclarationHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (EventDeclarationSyntax)context.Node;
            context.ReportDiagnostics(GenerateSpellingDiagnosticsForIdentifier(node.Identifier));
        }

        private void EventFieldDeclarationHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (EventFieldDeclarationSyntax)context.Node;
            foreach (var variableDeclaration in node.Declaration.Variables)
            {
                context.ReportDiagnostics(GenerateSpellingDiagnosticsForFieldIdentifier(variableDeclaration.Identifier));
            }
        }

        private void TypeParameterHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (TypeParameterSyntax)context.Node;
            context.ReportDiagnostics(GenerateSpellingDiagnosticsForGenericIdentifier(node.Identifier));
        }

        private void FromClauseHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (FromClauseSyntax)context.Node;
            context.ReportDiagnostics(GenerateSpellingDiagnosticsForGenericIdentifier(node.Identifier));
        }

        private void QueryContinuationHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (QueryContinuationSyntax)context.Node;
            context.ReportDiagnostics(GenerateSpellingDiagnosticsForGenericIdentifier(node.Identifier));
        }

        private void LetClauseHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (LetClauseSyntax)context.Node;
            context.ReportDiagnostics(GenerateSpellingDiagnosticsForGenericIdentifier(node.Identifier));
        }

        private void JoinClauseHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (JoinClauseSyntax)context.Node;
            context.ReportDiagnostics(GenerateSpellingDiagnosticsForGenericIdentifier(node.Identifier));
        }

        private void JoinIntoClauseHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (JoinIntoClauseSyntax)context.Node;
            context.ReportDiagnostics(GenerateSpellingDiagnosticsForGenericIdentifier(node.Identifier));
        }

        private void StringLiteralExpressionHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (LiteralExpressionSyntax)context.Node;
            var token = node.Token;

            var valueText = token.ValueText;
            var syntaxText = token.Text;

            var wordParser = new GeneralTextParser();
            var valueLocator = new StringLiteralSyntaxCharValueLocator(valueText, syntaxText, token.IsVerbatimStringLiteral());

            foreach (var part in wordParser.SplitWordParts(valueText).Where(part => part.IsWord))
            {
                if (!SpellChecker.Check(part.Text))
                {
                    var spellingStart = token.SpanStart + valueLocator.ConvertValueToSyntaxIndex(part.Start);
                    var location = Location.Create(
                        token.SyntaxTree,
                        TextSpan.FromBounds(
                            spellingStart,
                            spellingStart + part.Length));
                    var diagnostic = Diagnostic.Create(SpellingLiteralDiagnosticDescriptor, location, part.Text);

                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private void InterpolatedStringTextHandler(SyntaxNodeAnalysisContext context)
        {
            var node = (InterpolatedStringTextSyntax)context.Node;
            var token = node.TextToken;
            var valueText = token.ValueText;
            var syntaxText = token.Text;

            var wordParser = new GeneralTextParser();
            var valueLocator = new StringLiteralSyntaxCharValueLocator(valueText, syntaxText, token.IsVerbatimStringLiteral());

            foreach (var part in wordParser.SplitWordParts(valueText).Where(part => part.IsWord))
            {
                if (!SpellChecker.Check(part.Text))
                {
                    var spellingStart = token.SpanStart + valueLocator.ConvertValueToSyntaxIndex(part.Start);
                    var location = Location.Create(
                        token.SyntaxTree,
                        TextSpan.FromBounds(
                            spellingStart,
                            spellingStart + part.Length));
                    var diagnostic = Diagnostic.Create(SpellingLiteralDiagnosticDescriptor, location, part.Text);

                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            if (!context.Tree.HasCompilationUnitRoot)
            {
                return;
            }

            foreach (var node in context.Tree.GetCompilationUnitRoot(context.CancellationToken).DescendantTrivia())
            {
                switch (node.Kind())
                {
                    case SyntaxKind.SingleLineCommentTrivia:
                        SingleLineCommentTriviaHandler(node, context);
                        break;
                    case SyntaxKind.MultiLineCommentTrivia:
                        MultiLineCommentTriviaHandler(node);
                        break;
                    case SyntaxKind.SingleLineDocumentationCommentTrivia:
                    case SyntaxKind.MultiLineDocumentationCommentTrivia:
                        XmlTextLiteralTokenHandler(node);
                        break;
                }
            }
        }

        private void XmlTextLiteralTokenHandler(SyntaxTrivia node)
        {
            "".ToCharArray();
            ;
        }

        private void SingleLineCommentTriviaHandler(SyntaxTrivia node, SyntaxTreeAnalysisContext context)
        {
            var lineText = node.ToString();
            var textSpan = CommentTextExtractor.LocateSingleLineCommentText(lineText);
            if (textSpan.Length == 0)
            {
                return;
            }

            var wordParser = new GeneralTextParser();
            var parts = wordParser.SplitWordParts(lineText.Substring(textSpan.Start, textSpan.Length));
            foreach (var part in parts.Where(part => part.IsWord))
            {
                if (!SpellChecker.Check(part.Text))
                {
                    var spellingStart = node.SpanStart + textSpan.Start + part.Start;

                    var location = Location.Create(
                        node.SyntaxTree,
                        TextSpan.FromBounds(
                            spellingStart,
                            spellingStart + part.Length));
                    var diagnostic = Diagnostic.Create(CommentDiagnosticDescriptor, location, part.Text);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private void MultiLineCommentTriviaHandler(SyntaxTrivia node)
        {
            var allText = node.ToString();
            // TODO: extract all lines with spans
            // when extracting lines, ignore the leading `\s*[*]\s*` from other lines and the leading and trailing `[/][*]+` and `[*]+[/]`
            // while preserving span information
            ;
        }

        private IEnumerable<Diagnostic> GenerateSpellingDiagnosticsForIdentifier(SyntaxToken identifier)
        {
            var wordParser = new IdentifierWordParser();
            var parts = wordParser.SplitWordParts(identifier.Text);
            return GenerateSpellingDiagnosticsForWordParts(parts, identifier);
        }

        private IEnumerable<Diagnostic> GenerateSpellingDiagnosticsForInterfaceIdentifier(SyntaxToken identifier)
        {
            var wordParser = new IdentifierWordParser();
            var parts = wordParser.SplitWordParts(identifier.Text);
            parts = SkipFirstMatching(parts, "I");
            return GenerateSpellingDiagnosticsForWordParts(parts, identifier);
        }

        private IEnumerable<Diagnostic> GenerateSpellingDiagnosticsForGenericIdentifier(SyntaxToken identifier)
        {
            var wordParser = new IdentifierWordParser();
            var parts = wordParser.SplitWordParts(identifier.Text);
            parts = SkipFirstMatching(parts, "T");
            return GenerateSpellingDiagnosticsForWordParts(parts, identifier);
        }

        private IEnumerable<Diagnostic> GenerateSpellingDiagnosticsForFieldIdentifier(SyntaxToken identifier)
        {
            var wordParser = new IdentifierWordParser();
            var parts = wordParser.SplitWordParts(identifier.Text);
            parts = SkipFirstMatching(parts, "m");
            return GenerateSpellingDiagnosticsForWordParts(parts, identifier);
        }

        private IEnumerable<ParsedTextSpan> SkipFirstMatching(IEnumerable<ParsedTextSpan> parts, string firstSkipWord)
        {
            using (var enumerator = parts.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    yield break;
                }

                if (enumerator.Current.Text != firstSkipWord)
                {
                    yield return enumerator.Current;
                }

                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
            }
        }

        private IEnumerable<Diagnostic> GenerateSpellingDiagnosticsForWordParts(IEnumerable<ParsedTextSpan> parts, SyntaxToken identifier)
        {
            foreach (var part in parts.Where(part => part.IsWord))
            {
                if (!SpellChecker.Check(part.Text))
                {
                    var spellingStart = identifier.SpanStart + part.Start;

                    var location = Location.Create(
                        identifier.SyntaxTree,
                        TextSpan.FromBounds(
                            spellingStart,
                            spellingStart + part.Length));
                    yield return Diagnostic.Create(SpellingIdentifierDiagnosticDescriptor, location, part.Text);
                }
            }
        }
    }
}
