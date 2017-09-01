using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace WeCantSpell.Roslyn
{
    public class SpellCheckCSharpWalker : CSharpSyntaxWalker
    {
        public ISpellChecker SpellChecker { get; }

        public HashSet<string> VisitedWords { get; }

        public Action<SpellingMistake> MistakeHandler { get; }

        public SpellCheckCSharpWalker(
            ISpellChecker spellChecker,
            Action<SpellingMistake> mistakeHandler)
            : base(SyntaxWalkerDepth.StructuredTrivia)
        {
            SpellChecker = spellChecker ?? throw new ArgumentNullException(nameof(spellChecker));
            MistakeHandler = mistakeHandler ?? throw new ArgumentNullException(nameof(mistakeHandler));
            VisitedWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            FindSpellingMistakesForIdentifier(node.Identifier);
            base.VisitClassDeclaration(node);
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            FindSpellingMistakesForIdentifier(node.Identifier);
            base.VisitStructDeclaration(node);
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            FindSpellingMistakesForIdentifierSkippingFirstWord(node.Identifier, "I");
            base.VisitInterfaceDeclaration(node);
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            var declaration = node.Declaration;
            if (declaration != null)
            {
                foreach (var variable in declaration.Variables)
                {
                    FindSpellingMistakesForIdentifierSkippingFirstWord(variable.Identifier, "m");
                }
            }

            base.VisitFieldDeclaration(node);
        }

        public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            var declaration = node.Declaration;
            if (declaration != null)
            {
                foreach (var variable in declaration.Variables)
                {
                    FindSpellingMistakesForIdentifier(variable.Identifier);
                }
            }

            base.VisitLocalDeclarationStatement(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            FindSpellingMistakesForIdentifier(node.Identifier);
            base.VisitMethodDeclaration(node);
        }

        public override void VisitParameter(ParameterSyntax node)
        {
            FindSpellingMistakesForIdentifier(node.Identifier);
            base.VisitParameter(node);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            FindSpellingMistakesForIdentifier(node.Identifier);
            base.VisitPropertyDeclaration(node);
        }

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            var name = node.Alias?.Name;
            if (name != null)
            {
                FindSpellingMistakesForIdentifier(name.Identifier);
            }

            base.VisitUsingDirective(node);
        }

        public override void VisitUsingStatement(UsingStatementSyntax node)
        {
            var declaration = node.Declaration;
            if (declaration != null)
            {
                foreach (var variable in declaration.Variables)
                {
                    FindSpellingMistakesForIdentifier(variable.Identifier);
                }
            }
            base.VisitUsingStatement(node);
        }

        public override void VisitCatchDeclaration(CatchDeclarationSyntax node)
        {
            var identifier = node.Identifier;
            if (identifier.Span.Length != 0)
            {
                FindSpellingMistakesForIdentifier(identifier);
            }

            base.VisitCatchDeclaration(node);
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            FindSpellingMistakesForIdentifier(node.Identifier);
            base.VisitEnumDeclaration(node);
        }

        public override void VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
        {
            FindSpellingMistakesForIdentifier(node.Identifier);
            base.VisitEnumMemberDeclaration(node);
        }

        public override void VisitAnonymousObjectMemberDeclarator(AnonymousObjectMemberDeclaratorSyntax node)
        {
            var name = node.NameEquals?.Name;
            if (name != null)
            {
                FindSpellingMistakesForIdentifier(name.Identifier);
            }

            base.VisitAnonymousObjectMemberDeclarator(node);
        }

        public override void VisitForEachStatement(ForEachStatementSyntax node)
        {
            FindSpellingMistakesForIdentifier(node.Identifier);
            base.VisitForEachStatement(node);
        }

        public override void VisitForStatement(ForStatementSyntax node)
        {
            var declaration = node.Declaration;
            if (declaration != null)
            {
                foreach (var variable in declaration.Variables)
                {
                    FindSpellingMistakesForIdentifier(variable.Identifier);
                }
            }

            base.VisitForStatement(node);
        }

        public override void VisitLabeledStatement(LabeledStatementSyntax node)
        {
            FindSpellingMistakesForIdentifier(node.Identifier);
            base.VisitLabeledStatement(node);
        }

        public override void VisitDelegateDeclaration(DelegateDeclarationSyntax node)
        {
            FindSpellingMistakesForIdentifier(node.Identifier);
            base.VisitDelegateDeclaration(node);
        }

        public override void VisitEventDeclaration(EventDeclarationSyntax node)
        {
            FindSpellingMistakesForIdentifier(node.Identifier);
            base.VisitEventDeclaration(node);
        }

        public override void VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
            var declaration = node.Declaration;
            if (declaration != null)
            {
                foreach(var variable in declaration.Variables)
                {
                    FindSpellingMistakesForIdentifier(variable.Identifier);
                }
            }
            base.VisitEventFieldDeclaration(node);
        }

        public override void VisitTypeParameter(TypeParameterSyntax node)
        {
            FindSpellingMistakesForIdentifierSkippingFirstWord(node.Identifier, "T");
            base.VisitTypeParameter(node);
        }

        public override void VisitFromClause(FromClauseSyntax node)
        {
            FindSpellingMistakesForIdentifier(node.Identifier);
            base.VisitFromClause(node);
        }

        public override void VisitQueryContinuation(QueryContinuationSyntax node)
        {
            FindSpellingMistakesForIdentifier(node.Identifier);
            base.VisitQueryContinuation(node);
        }

        public override void VisitLetClause(LetClauseSyntax node)
        {
            FindSpellingMistakesForIdentifier(node.Identifier);
            base.VisitLetClause(node);
        }

        public override void VisitJoinClause(JoinClauseSyntax node)
        {
            FindSpellingMistakesForIdentifier(node.Identifier);
            base.VisitJoinClause(node);
        }

        public override void VisitJoinIntoClause(JoinIntoClauseSyntax node)
        {
            FindSpellingMistakesForIdentifier(node.Identifier);
            base.VisitJoinIntoClause(node);
        }

        public override void VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            var token = node.Token;
            var valueText = token.ValueText;
            var syntaxText = token.Text;
            var valueLocator = new StringLiteralSyntaxCharValueLocator(valueText, syntaxText, token.IsVerbatimStringLiteral());

            var parts = GeneralTextParser.SplitWordParts(valueText);
            foreach (var part in parts)
            {
                if (ShouldWordBeMarkedAsMisspelled(part))
                {
                    var spanOffset = valueLocator.ConvertValueToSyntaxIndex(part.Start);
                    var location = Location.Create(token.SyntaxTree, new TextSpan(token.SpanStart + spanOffset, part.Length));
                    HandleMistake(location, part.Text, SpellingMistakeKind.Literal);
                }
            }

            base.VisitLiteralExpression(node);
        }

        public override void VisitInterpolatedStringText(InterpolatedStringTextSyntax node)
        {
            var token = node.TextToken;
            var valueText = token.ValueText;
            var syntaxText = token.Text;
            var valueLocator = new StringLiteralSyntaxCharValueLocator(valueText, syntaxText, token.IsVerbatimStringLiteral());

            var parts = GeneralTextParser.SplitWordParts(valueText);
            foreach (var part in parts)
            {
                if (ShouldWordBeMarkedAsMisspelled(part))
                {
                    var spanOffset = valueLocator.ConvertValueToSyntaxIndex(part.Start);
                    var location = Location.Create(token.SyntaxTree, new TextSpan(token.SpanStart + spanOffset, part.Length));
                    HandleMistake(location, part.Text, SpellingMistakeKind.Literal);
                }
            }

            base.VisitInterpolatedStringText(node);
        }

        public override void VisitTrivia(SyntaxTrivia trivia)
        {
            FindSpellingMistakesInTrivia(trivia);
            base.VisitTrivia(trivia);
        }

        public override void VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node)
        {
            foreach (var xmlNode in node.Content)
            {
                FindSpellingMistakesInXmlNodeSyntax(xmlNode);
            }

            base.VisitDocumentationCommentTrivia(node);
        }

        bool ShouldWordBeMarkedAsMisspelled(ParsedTextSpan part) => part.IsWord && !SpellChecker.Check(part.Text);

        void FindSpellingMistakesForIdentifier(SyntaxToken identifier)
        {
            var wordParts = IdentifierWordParser.SplitWordParts(identifier.Text);
            FindSpellingMistakesForIdentifierWordParts(wordParts, identifier);
        }

        void FindSpellingMistakesForIdentifierSkippingFirstWord(SyntaxToken identifier, string firstSkipWord)
        {
            var wordParts = IdentifierWordParser.SplitWordParts(identifier.Text);
            FindSpellingMistakesForIdentifierWordParts(wordParts, identifier, firstSkipWord);
        }

        void FindSpellingMistakesInTrivia(SyntaxTrivia trivia)
        {
            var kind = trivia.Kind();
            if (kind == SyntaxKind.SingleLineCommentTrivia)
            {
                FindSpellingMistakesInSingleLineComment(trivia);
            }
            else if (kind == SyntaxKind.MultiLineCommentTrivia)
            {
                FindSpellingMistakesInMultiLineComment(trivia);
            }
        }

        void FindSpellingMistakesInSingleLineComment(SyntaxTrivia node)
        {
            var lineText = node.ToString();
            var textSpan = CommentTextExtractor.LocateSingleLineCommentText(lineText);
            if (textSpan.Length == 0)
            {
                return;
            }

            var parts = GeneralTextParser.SplitWordParts(lineText.Substring(textSpan.Start, textSpan.Length));
            foreach (var part in parts)
            {
                if (ShouldWordBeMarkedAsMisspelled(part))
                {
                    var location = Location.Create(node.SyntaxTree, new TextSpan(node.SpanStart + textSpan.Start + part.Start, part.Length));
                    HandleMistake(location, part.Text, SpellingMistakeKind.Comment);
                }
            }
        }

        void FindSpellingMistakesInMultiLineComment(SyntaxTrivia node)
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
                foreach (var part in wordParts)
                {
                    if (ShouldWordBeMarkedAsMisspelled(part))
                    {
                        var location = Location.Create(node.SyntaxTree, new TextSpan(node.SpanStart + lineTextSpan.Start + part.Start, part.Length));
                        HandleMistake(location, part.Text, SpellingMistakeKind.Comment);
                    }
                }
            }
        }

        void FindSpellingMistakesInXmlNodeSyntax(XmlNodeSyntax node)
        {
            if (node is XmlElementSyntax xmlElementSyntax)
            {
                var localName = xmlElementSyntax.StartTag.Name.ToString();
                if (localName != "c" && localName != "code")
                {
                    foreach (var childNode in xmlElementSyntax.Content)
                    {
                        FindSpellingMistakesInXmlNodeSyntax(childNode);
                    }
                }
            }
            else if (node is XmlTextSyntax xmlTextSyntax)
            {
                FindSpellingMistakesInXmlTextSyntax(xmlTextSyntax);
            }
        }

        void FindSpellingMistakesInXmlTextSyntax(XmlTextSyntax node)
        {
            var allText = node.ToString();
            var lineTextSpans = CommentTextExtractor.LocateMultiLineCommentTextParts(allText);

            foreach (var lineTextSpan in lineTextSpans)
            {
                var lineText = allText.Substring(lineTextSpan.Start, lineTextSpan.Length);
                var wordParts = GeneralTextParser.SplitWordParts(lineText);
                foreach (var part in wordParts)
                {
                    if (ShouldWordBeMarkedAsMisspelled(part))
                    {
                        var location = Location.Create(node.SyntaxTree, new TextSpan(node.SpanStart + lineTextSpan.Start + part.Start, part.Length));
                        HandleMistake(location, part.Text, SpellingMistakeKind.Documentation);
                    }
                }
            }
        }

        void FindSpellingMistakesForIdentifierWordParts(List<ParsedTextSpan> parts, SyntaxToken identifier, string firstSkipWord = null)
        {
            if (parts.Count == 0)
            {
                return;
            }

            var part = parts[0];
            if (firstSkipWord == null || part.Text != firstSkipWord)
            {
                if (ShouldWordBeMarkedAsMisspelled(part))
                {
                    addMistake(ref part);
                }
            }

            for(var i = 1; i < parts.Count; i++)
            {
                part = parts[i];
                if (ShouldWordBeMarkedAsMisspelled(part))
                {
                    addMistake(ref part);
                }
            }

            void addMistake(ref ParsedTextSpan givenPart)
            {
                var location = Location.Create(identifier.SyntaxTree, new TextSpan(identifier.SpanStart + givenPart.Start, givenPart.Length));
                HandleMistake(location, givenPart.Text, SpellingMistakeKind.Identifier);
            }
        }

        void HandleMistake(Location location, string text, SpellingMistakeKind kind) =>
            MistakeHandler(new SpellingMistake(location, text, kind));
    }
}
