using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using WeCantSpell.Hunspell;

namespace WeCantSpell.Roslyn
{
    public class EmbeddedSpellChecker : ISpellChecker
    {
        private static WordList Load(string languageCode)
        {
            const string resourceNamespaceBase = "";
            string languageResourceName = resourceNamespaceBase + languageCode;
            string affName = languageResourceName + ".aff.gz";
            string dicName = languageResourceName + ".dic.gz";

            Assembly assembly = typeof(EmbeddedSpellChecker).GetTypeInfo().Assembly;

            using Stream affCompressedStream = assembly.GetManifestResourceStream(affName) ?? throw new InvalidOperationException();
            using var affStream = new GZipStream(affCompressedStream, CompressionMode.Decompress);
            using Stream dicCompressedStream = assembly.GetManifestResourceStream(dicName) ?? throw new InvalidOperationException();
            using var dicStream = new GZipStream(dicCompressedStream, CompressionMode.Decompress);
            return WordList.CreateFromStreams(dicStream, affStream);
        }

        public EmbeddedSpellChecker(string languageCode)
        {
            // LanguageCode = languageCode ?? throw new ArgumentNullException(nameof(languageCode));
            WordLists.Add(Load(languageCode));
        }

        public EmbeddedSpellChecker(string[] languageCodes)
        {
            // LanguageCode = languageCode ?? throw new ArgumentNullException(nameof(languageCode));
            foreach (var languageCode in languageCodes)
            {
                WordLists.Add(Load(languageCode));
            }
        }

        // public string LanguageCode { get; }

        private List<WordList> WordLists { get; } = new();

        public bool Check(string word)
        {
            return WordLists.Any(wordList => wordList.Check(word));
        }

        public IEnumerable<string> Suggest(string word)
        {
            return WordLists.SelectMany(wordList => wordList.Suggest(word));
        }
    }
}
