using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.Text;

namespace WeCantSpell
{
    public class SpellCheckWalker : CSharpSyntaxWalker
    {
        public ISpellChecker SpellChecker { get; }

        public HashSet<string> VisitedWords { get; }

        public List<SpellingMistake> Mistakes { get; }

        public SpellCheckWalker(ISpellChecker spellChecker)
            : base(SyntaxWalkerDepth.StructuredTrivia)
        {
            SpellChecker = spellChecker;
            VisitedWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            Mistakes = new List<SpellingMistake>();
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            AddSpellingMistakes(GenerateSpellingMistakesForIdentifier(node.Identifier));
            base.VisitClassDeclaration(node);
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            AddSpellingMistakes(GenerateSpellingMistakesForIdentifier(node.Identifier));
            base.VisitStructDeclaration(node);
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            AddSpellingMistakes(GenerateSpellingMistakesForIdentifierSkippingFirstWord(node.Identifier, "I"));
            base.VisitInterfaceDeclaration(node);
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            var declaration = node.Declaration;
            if (declaration != null)
            {
                AddSpellingMistakes(
                    declaration.Variables
                        .SelectMany(v => GenerateSpellingMistakesForIdentifierSkippingFirstWord(v.Identifier, "m")));
            }
            base.VisitFieldDeclaration(node);
        }

        public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            var declaration = node.Declaration;
            if (declaration != null)
            {
                AddSpellingMistakes(
                    declaration.Variables
                        .SelectMany(v => GenerateSpellingMistakesForIdentifier(v.Identifier)));
            }
            base.VisitLocalDeclarationStatement(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            AddSpellingMistakes(GenerateSpellingMistakesForIdentifier(node.Identifier));
            base.VisitMethodDeclaration(node);
        }

        public override void VisitParameter(ParameterSyntax node)
        {
            AddSpellingMistakes(GenerateSpellingMistakesForIdentifier(node.Identifier));
            base.VisitParameter(node);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            AddSpellingMistakes(GenerateSpellingMistakesForIdentifier(node.Identifier));
            base.VisitPropertyDeclaration(node);
        }

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            var name = node.Alias?.Name;
            if (name != null)
            {
                AddSpellingMistakes(GenerateSpellingMistakesForIdentifier(name.Identifier));
            }

            base.VisitUsingDirective(node);
        }

        public override void VisitUsingStatement(UsingStatementSyntax node)
        {
            var declaration = node.Declaration;
            if (declaration != null)
            {
                AddSpellingMistakes(
                    declaration.Variables
                        .SelectMany(v => GenerateSpellingMistakesForIdentifier(v.Identifier)));
            }
            base.VisitUsingStatement(node);
        }

        public override void VisitCatchDeclaration(CatchDeclarationSyntax node)
        {
            var identifier = node.Identifier;
            if (identifier.Span.Length != 0)
            {
                AddSpellingMistakes(GenerateSpellingMistakesForIdentifier(identifier));
            }

            base.VisitCatchDeclaration(node);
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            AddSpellingMistakes(GenerateSpellingMistakesForIdentifier(node.Identifier));
            base.VisitEnumDeclaration(node);
        }

        public override void VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
        {
            AddSpellingMistakes(GenerateSpellingMistakesForIdentifier(node.Identifier));
            base.VisitEnumMemberDeclaration(node);
        }

        public override void VisitAnonymousObjectMemberDeclarator(AnonymousObjectMemberDeclaratorSyntax node)
        {
            var name = node.NameEquals?.Name;
            if (name != null)
            {
                AddSpellingMistakes(GenerateSpellingMistakesForIdentifier(name.Identifier));
            }

            base.VisitAnonymousObjectMemberDeclarator(node);
        }

        public override void VisitForEachStatement(ForEachStatementSyntax node)
        {
            AddSpellingMistakes(GenerateSpellingMistakesForIdentifier(node.Identifier));
            base.VisitForEachStatement(node);
        }

        public override void VisitForStatement(ForStatementSyntax node)
        {
            var declaration = node.Declaration;
            if (declaration != null)
            {
                AddSpellingMistakes(
                    declaration.Variables
                        .SelectMany(v => GenerateSpellingMistakesForIdentifier(v.Identifier)));
            }
            base.VisitForStatement(node);
        }

        public override void VisitLabeledStatement(LabeledStatementSyntax node)
        {
            AddSpellingMistakes(GenerateSpellingMistakesForIdentifier(node.Identifier));
            base.VisitLabeledStatement(node);
        }

        public override void VisitDelegateDeclaration(DelegateDeclarationSyntax node)
        {
            AddSpellingMistakes(GenerateSpellingMistakesForIdentifier(node.Identifier));
            base.VisitDelegateDeclaration(node);
        }

        public override void VisitEventDeclaration(EventDeclarationSyntax node)
        {
            AddSpellingMistakes(GenerateSpellingMistakesForIdentifier(node.Identifier));
            base.VisitEventDeclaration(node);
        }

        public override void VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
            var declaration = node.Declaration;
            if (declaration != null)
            {
                AddSpellingMistakes(
                    declaration.Variables
                        .SelectMany(v => GenerateSpellingMistakesForIdentifier(v.Identifier)));
            }
            base.VisitEventFieldDeclaration(node);
        }

        public override void VisitTypeParameter(TypeParameterSyntax node)
        {
            AddSpellingMistakes(GenerateSpellingMistakesForIdentifierSkippingFirstWord(node.Identifier, "T"));
            base.VisitTypeParameter(node);
        }

        public override void VisitFromClause(FromClauseSyntax node)
        {
            AddSpellingMistakes(GenerateSpellingMistakesForIdentifier(node.Identifier));
            base.VisitFromClause(node);
        }

        public override void VisitQueryContinuation(QueryContinuationSyntax node)
        {
            AddSpellingMistakes(GenerateSpellingMistakesForIdentifier(node.Identifier));
            base.VisitQueryContinuation(node);
        }

        public override void VisitLetClause(LetClauseSyntax node)
        {
            AddSpellingMistakes(GenerateSpellingMistakesForIdentifier(node.Identifier));
            base.VisitLetClause(node);
        }

        public override void VisitJoinClause(JoinClauseSyntax node)
        {
            AddSpellingMistakes(GenerateSpellingMistakesForIdentifier(node.Identifier));
            base.VisitJoinClause(node);
        }

        public override void VisitJoinIntoClause(JoinIntoClauseSyntax node)
        {
            AddSpellingMistakes(GenerateSpellingMistakesForIdentifier(node.Identifier));
            base.VisitJoinIntoClause(node);
        }

        public override void VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            var token = node.Token;
            var valueText = token.ValueText;
            var syntaxText = token.Text;
            var valueLocator = new StringLiteralSyntaxCharValueLocator(valueText, syntaxText, token.IsVerbatimStringLiteral());

            AddSpellingMistakes(
                GeneralTextParser.SplitWordParts(valueText)
                    .Where(part => part.IsWord && ShouldWordBeMarkedAsMisspelled(part.Text))
                    .Select(part =>
                    {
                        var spellingStart = token.SpanStart + valueLocator.ConvertValueToSyntaxIndex(part.Start);
                        var location = Location.Create(
                            token.SyntaxTree,
                            TextSpan.FromBounds(
                                spellingStart,
                                spellingStart + part.Length));

                        return new SpellingMistake(location, part.Text, SpellingMistakeKind.Literal);
                    }));

            base.VisitLiteralExpression(node);
        }

        public override void VisitInterpolatedStringText(InterpolatedStringTextSyntax node)
        {
            var token = node.TextToken;
            var valueText = token.ValueText;
            var syntaxText = token.Text;
            var valueLocator = new StringLiteralSyntaxCharValueLocator(valueText, syntaxText, token.IsVerbatimStringLiteral());

            AddSpellingMistakes(
                GeneralTextParser.SplitWordParts(valueText)
                    .Where(part => part.IsWord && ShouldWordBeMarkedAsMisspelled(part.Text))
                    .Select(part =>
                    {
                        var spellingStart = token.SpanStart + valueLocator.ConvertValueToSyntaxIndex(part.Start);
                        var location = Location.Create(
                            token.SyntaxTree,
                            TextSpan.FromBounds(
                                spellingStart,
                                spellingStart + part.Length));

                        return new SpellingMistake(location, part.Text, SpellingMistakeKind.Literal);
                    }));

            base.VisitInterpolatedStringText(node);
        }

        public override void VisitTrivia(SyntaxTrivia trivia)
        {
            AddSpellingMistakes(FindSpellingMistakesInTrivia(trivia));
            base.VisitTrivia(trivia);
        }

        public override void VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node)
        {
            foreach (var xmlNode in node.Content)
            {
                AddSpellingMistakes(FindSpellingMistakesInXmlNodeSyntax(xmlNode));
            }

            base.VisitDocumentationCommentTrivia(node);
        }

        private bool ShouldWordBeMarkedAsMisspelled(string word) => !SpellChecker.Check(word);

        private void AddSpellingMistakes(IEnumerable<SpellingMistake> mistakes) =>
            Mistakes.AddRange(mistakes);

        private IEnumerable<SpellingMistake> GenerateSpellingMistakesForIdentifier(SyntaxToken identifier) =>
            GenerateSpellingMistakesForIdentifierWordParts(
                IdentifierWordParser.SplitWordParts(identifier.Text),
                identifier);

        private IEnumerable<SpellingMistake> GenerateSpellingMistakesForIdentifierSkippingFirstWord(SyntaxToken identifier, string firstSkipWord) =>
            GenerateSpellingMistakesForIdentifierWordParts(
                SkipFirstMatching(
                    IdentifierWordParser.SplitWordParts(identifier.Text),
                    firstSkipWord),
                identifier);

        private IEnumerable<SpellingMistake> FindSpellingMistakesInTrivia(SyntaxTrivia trivia)
        {
            var kind = trivia.Kind();
            if (kind == SyntaxKind.SingleLineCommentTrivia)
            {
                return FindSpellingMistakesInSingleLineComment(trivia);
            }
            else if (kind == SyntaxKind.MultiLineCommentTrivia)
            {
                return FindSpellingMistakesInMultiLineComment(trivia);
            }
            return Enumerable.Empty<SpellingMistake>();
        }

        private IEnumerable<SpellingMistake> FindSpellingMistakesInSingleLineComment(SyntaxTrivia node)
        {
            var lineText = node.ToString();
            var textSpan = CommentTextExtractor.LocateSingleLineCommentText(lineText);
            if (textSpan.Length == 0)
            {
                yield break;
            }

            var parts = GeneralTextParser.SplitWordParts(lineText.Substring(textSpan.Start, textSpan.Length));
            foreach (var part in parts.Where(part => part.IsWord && ShouldWordBeMarkedAsMisspelled(part.Text)))
            {
                var spellingStart = node.SpanStart + textSpan.Start + part.Start;

                var location = Location.Create(
                    node.SyntaxTree,
                    TextSpan.FromBounds(
                        spellingStart,
                        spellingStart + part.Length));

                yield return new SpellingMistake(location, part.Text, SpellingMistakeKind.Comment);
            }
        }

        private IEnumerable<SpellingMistake> FindSpellingMistakesInMultiLineComment(SyntaxTrivia node)
        {
            var allText = node.ToString();
            var lineTextSpans = CommentTextExtractor.LocateMultiLineCommentTextParts(allText);

            foreach (var lineTextSpan in lineTextSpans)
            {
                if (lineTextSpan.Length == 0)
                {
                    continue;
                }

                var lineText = allText.Substring(lineTextSpan.Start, lineTextSpan.Length);
                var wordParts = GeneralTextParser.SplitWordParts(lineText);
                foreach (var wordPart in wordParts.Where(part => part.IsWord && ShouldWordBeMarkedAsMisspelled(part.Text)))
                {
                    var spellingStart = node.SpanStart + lineTextSpan.Start + wordPart.Start;
                    var location = Location.Create(
                        node.SyntaxTree,
                        TextSpan.FromBounds(
                            spellingStart,
                            spellingStart + wordPart.Length));

                    yield return new SpellingMistake(location, wordPart.Text, SpellingMistakeKind.Comment);
                }
            }
        }

        private IEnumerable<SpellingMistake> FindSpellingMistakesInXmlNodeSyntax(XmlNodeSyntax node)
        {
            if (node is XmlElementSyntax xmlElementSyntax)
            {
                var localName = xmlElementSyntax.StartTag.Name.ToString();
                if (localName != "c" && localName != "code")
                {
                    return xmlElementSyntax.Content.SelectMany(FindSpellingMistakesInXmlNodeSyntax);
                }
            }
            else if (node is XmlTextSyntax xmlTextSyntax)
            {
                return FindSpellingMistakesInXmlTextSyntax(xmlTextSyntax);
            }

            return Enumerable.Empty<SpellingMistake>();
        }

        private IEnumerable<SpellingMistake> FindSpellingMistakesInXmlTextSyntax(XmlTextSyntax node)
        {
            var allText = node.ToString();
            var lineTextSpans = CommentTextExtractor.LocateMultiLineCommentTextParts(allText);

            foreach (var lineTextSpan in lineTextSpans)
            {
                var lineText = allText.Substring(lineTextSpan.Start, lineTextSpan.Length);
                var wordParts = GeneralTextParser.SplitWordParts(lineText);
                foreach (var wordPart in wordParts.Where(part => part.IsWord && ShouldWordBeMarkedAsMisspelled(part.Text)))
                {
                    var spellingStart = node.SpanStart + lineTextSpan.Start + wordPart.Start;
                    var location = Location.Create(
                        node.SyntaxTree,
                        TextSpan.FromBounds(
                            spellingStart,
                            spellingStart + wordPart.Length));

                    yield return new SpellingMistake(location, wordPart.Text, SpellingMistakeKind.Documentation);
                }
            }
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

        private IEnumerable<SpellingMistake> GenerateSpellingMistakesForIdentifierWordParts(IEnumerable<ParsedTextSpan> parts, SyntaxToken identifier) =>
            parts
                .Where(part => part.IsWord && ShouldWordBeMarkedAsMisspelled(part.Text))
                .Select(part =>
                {
                    var spellingStart = identifier.SpanStart + part.Start;
                    var location = Location.Create(
                        identifier.SyntaxTree,
                        TextSpan.FromBounds(
                            spellingStart,
                            spellingStart + part.Length));

                    return new SpellingMistake(location, part.Text, SpellingMistakeKind.Identifier);
                });
    }
}
