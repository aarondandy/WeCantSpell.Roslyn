using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using WeCantSpell.Roslyn.Config;
using WeCantSpell.Roslyn.Infrastructure;

namespace WeCantSpell.Roslyn
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SpellingCodeFixProviderCSharp))]
    [UsedImplicitly]
    public class SpellingCodeFixProviderCSharp : CodeFixProvider
    {
        private readonly IFileSystem _fileSystem;

        public SpellingCodeFixProviderCSharp()
            : this(new FileSystem()) { }

        public SpellingCodeFixProviderCSharp(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxTree = await context.Document.GetSyntaxTreeAsync(context.CancellationToken).ConfigureAwait(false);

            // Get the SourceText from the syntax tree
            if (syntaxTree == null)
                throw new InvalidOperationException("Can't get document syntax tree");

            var sourceText = await syntaxTree.GetTextAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            // Extract the text from the diagnostic's SourceSpan
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var diagnosticText = sourceText.GetSubText(diagnosticSpan).ToString();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: $"Add '{diagnosticText}' to project dictionary",
                    createChangedDocument: async _ => await AddToDictionaryFixAsync(context.Document, diagnosticText),
                    equivalenceKey: nameof(SpellingCodeFixProviderCSharp)
                ),
                diagnostic
            );
        }

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        private async Task<Document> AddToDictionaryFixAsync(Document document, string dictionaryWord)
        {
            if (document.FilePath == null)
            {
                throw new InvalidOperationException("Document has no FilePath, no project dictionary can be found");
            }

            var options = new SpellCheckerOptions(_fileSystem, document.FilePath);
            var updater = SpellCheckerPool.Shared.GetUpdater(options);
            await updater.AddToLocalDictionaryAsync(dictionaryWord).ConfigureAwait(false);

            return document;
        }

        public override ImmutableArray<string> FixableDiagnosticIds => SpellingAnalyzerCSharp.SupportedDiagnosticIds;
    }
}
