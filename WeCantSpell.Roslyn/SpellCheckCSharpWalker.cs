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
            VariableDeclarationSyntax declaration = node.Declaration;
            if (declaration != null)
            {
                foreach (VariableDeclaratorSyntax variable in declaration.Variables)
                {
                    FindSpellingMistakesForIdentifierSkippingFirstWord(variable.Identifier, "m");
                }
            }

            base.VisitFieldDeclaration(node);
        }

        public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            VariableDeclarationSyntax declaration = node.Declaration;
            if (declaration != null)
            {
                foreach (VariableDeclaratorSyntax variable in declaration.Variables)
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
            IdentifierNameSyntax? name = node.Alias?.Name;
            if (name != null)
            {
                FindSpellingMistakesForIdentifier(name.Identifier);
            }

            base.VisitUsingDirective(node);
        }

        public override void VisitUsingStatement(UsingStatementSyntax node)
        {
            VariableDeclarationSyntax? declaration = node.Declaration;
            if (declaration != null)
            {
                foreach (VariableDeclaratorSyntax variable in declaration.Variables)
                {
                    FindSpellingMistakesForIdentifier(variable.Identifier);
                }
            }
            base.VisitUsingStatement(node);
        }

        public override void VisitCatchDeclaration(CatchDeclarationSyntax node)
        {
            SyntaxToken identifier = node.Identifier;
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
            IdentifierNameSyntax? name = node.NameEquals?.Name;
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
            VariableDeclarationSyntax? declaration = node.Declaration;
            if (declaration != null)
            {
                foreach (VariableDeclaratorSyntax variable in declaration.Variables)
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
            VariableDeclarationSyntax declaration = node.Declaration;
            if (declaration != null)
            {
                foreach(VariableDeclaratorSyntax variable in declaration.Variables)
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
            SyntaxToken token = node.Token;
            string valueText = token.ValueText;
            string syntaxText = token.Text;
            var valueLocator = new StringLiteralSyntaxCharValueLocator(valueText, syntaxText, token.IsVerbatimStringLiteral());

            List<ParsedTextSpan> parts = GeneralTextParser.SplitWordParts(valueText);
            foreach (ParsedTextSpan part in parts)
            {
                if (ShouldWordBeMarkedAsMisspelled(part))
                {
                    int spanOffset = valueLocator.ConvertValueToSyntaxIndex(part.Start);
                    if (token.SyntaxTree != null)
                    {
                        var location = Location.Create(token.SyntaxTree, new TextSpan(token.SpanStart + spanOffset, part.Length));
                        HandleMistake(location, part.Text, SpellingMistakeKind.Literal);
                    }
                }
            }

            base.VisitLiteralExpression(node);
        }

        public override void VisitInterpolatedStringText(InterpolatedStringTextSyntax node)
        {
            SyntaxToken token = node.TextToken;
            string valueText = token.ValueText;
            string syntaxText = token.Text;
            var valueLocator = new StringLiteralSyntaxCharValueLocator(valueText, syntaxText, token.IsVerbatimStringLiteral());

            List<ParsedTextSpan> parts = GeneralTextParser.SplitWordParts(valueText);
            foreach (ParsedTextSpan part in parts)
            {
                if (ShouldWordBeMarkedAsMisspelled(part))
                {
                    int spanOffset = valueLocator.ConvertValueToSyntaxIndex(part.Start);
                    if (token.SyntaxTree != null)
                    {
                        var location = Location.Create(token.SyntaxTree, new TextSpan(token.SpanStart + spanOffset, part.Length));
                        HandleMistake(location, part.Text, SpellingMistakeKind.Literal);
                    }
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
            foreach (XmlNodeSyntax xmlNode in node.Content)
            {
                FindSpellingMistakesInXmlNodeSyntax(xmlNode);
            }

            base.VisitDocumentationCommentTrivia(node);
        }

        private bool ShouldWordBeMarkedAsMisspelled(ParsedTextSpan part) => part.IsWord && !SpellChecker.Check(part.Text);

        private void FindSpellingMistakesForIdentifier(SyntaxToken identifier)
        {
            List<ParsedTextSpan> wordParts = IdentifierWordParser.SplitWordParts(identifier.Text);
            FindSpellingMistakesForIdentifierWordParts(wordParts, identifier);
        }

        private void FindSpellingMistakesForIdentifierSkippingFirstWord(SyntaxToken identifier, string? firstSkipWord)
        {
            List<ParsedTextSpan> wordParts = IdentifierWordParser.SplitWordParts(identifier.Text);
            FindSpellingMistakesForIdentifierWordParts(wordParts, identifier, firstSkipWord);
        }

        private void FindSpellingMistakesInTrivia(SyntaxTrivia trivia)
        {
            SyntaxKind kind = trivia.Kind();
            if (kind == SyntaxKind.SingleLineCommentTrivia)
            {
                FindSpellingMistakesInSingleLineComment(trivia);
            }
            else if (kind == SyntaxKind.MultiLineCommentTrivia)
            {
                FindSpellingMistakesInMultiLineComment(trivia);
            }
        }

        private void FindSpellingMistakesInSingleLineComment(SyntaxTrivia node)
        {
            var lineText = node.ToString();
            TextSpan textSpan = CommentTextExtractor.LocateSingleLineCommentText(lineText);
            if (textSpan.Length == 0)
            {
                return;
            }

            List<ParsedTextSpan> parts = GeneralTextParser.SplitWordParts(lineText.Substring(textSpan.Start, textSpan.Length));
            foreach (ParsedTextSpan part in parts)
            {
                if (ShouldWordBeMarkedAsMisspelled(part))
                {
                    if (node.SyntaxTree != null)
                    {
                        var location = Location.Create(node.SyntaxTree, new TextSpan(node.SpanStart + textSpan.Start + part.Start, part.Length));
                        HandleMistake(location, part.Text, SpellingMistakeKind.Comment);
                    }
                }
            }
        }

        private void FindSpellingMistakesInMultiLineComment(SyntaxTrivia node)
        {
            var allText = node.ToString();
            List<TextSpan> lineTextSpans = CommentTextExtractor.LocateMultiLineCommentTextParts(allText);

            foreach (TextSpan lineTextSpan in lineTextSpans)
            {
                if (lineTextSpan.Length == 0)
                {
                    continue;
                }

                string lineText = allText.Substring(lineTextSpan.Start, lineTextSpan.Length);
                List<ParsedTextSpan> wordParts = GeneralTextParser.SplitWordParts(lineText);
                foreach (ParsedTextSpan part in wordParts)
                {
                    if (ShouldWordBeMarkedAsMisspelled(part))
                    {
                        if (node.SyntaxTree != null)
                        {
                            var location = Location.Create(node.SyntaxTree, new TextSpan(node.SpanStart + lineTextSpan.Start + part.Start, part.Length));
                            HandleMistake(location, part.Text, SpellingMistakeKind.Comment);
                        }
                    }
                }
            }
        }

        private void FindSpellingMistakesInXmlNodeSyntax(XmlNodeSyntax node)
        {
            if (node is XmlElementSyntax xmlElementSyntax)
            {
                var localName = xmlElementSyntax.StartTag.Name.ToString();
                if (localName != "c" && localName != "code")
                {
                    foreach (XmlNodeSyntax childNode in xmlElementSyntax.Content)
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

        private void FindSpellingMistakesInXmlTextSyntax(XmlTextSyntax node)
        {
            var allText = node.ToString();
            List<TextSpan> lineTextSpans = CommentTextExtractor.LocateMultiLineCommentTextParts(allText);

            foreach (TextSpan lineTextSpan in lineTextSpans)
            {
                string lineText = allText.Substring(lineTextSpan.Start, lineTextSpan.Length);
                List<ParsedTextSpan> wordParts = GeneralTextParser.SplitWordParts(lineText);
                foreach (ParsedTextSpan part in wordParts)
                {
                    if (ShouldWordBeMarkedAsMisspelled(part))
                    {
                        var location = Location.Create(node.SyntaxTree, new TextSpan(node.SpanStart + lineTextSpan.Start + part.Start, part.Length));
                        HandleMistake(location, part.Text, SpellingMistakeKind.Documentation);
                    }
                }
            }
        }

        private void FindSpellingMistakesForIdentifierWordParts(List<ParsedTextSpan> parts, SyntaxToken identifier, string? firstSkipWord = null)
        {
            if (parts.Count == 0)
            {
                return;
            }

            ParsedTextSpan part = parts[0];
            if (firstSkipWord == null || part.Text != firstSkipWord)
            {
                if (ShouldWordBeMarkedAsMisspelled(part))
                {
                    AddMistake(ref part);
                }
            }

            for(var i = 1; i < parts.Count; i++)
            {
                part = parts[i];
                if (ShouldWordBeMarkedAsMisspelled(part))
                {
                    AddMistake(ref part);
                }
            }

            void AddMistake(ref ParsedTextSpan givenPart)
            {
                if (identifier.SyntaxTree != null)
                {
                    var location = Location.Create(identifier.SyntaxTree, new TextSpan(identifier.SpanStart + givenPart.Start, givenPart.Length));
                    HandleMistake(location, givenPart.Text, SpellingMistakeKind.Identifier);
                }
            }
        }

        private void HandleMistake(Location location, string text, SpellingMistakeKind kind) =>
            MistakeHandler(new SpellingMistake(location, text, kind));
    }
}
