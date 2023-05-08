using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using WeCantSpell.Hunspell;
using WeCantSpell.Roslyn.Infrastructure;

namespace WeCantSpell.Roslyn
{
    /// <summary>
    /// Spell checker with dictionaries embedded in resources
    /// </summary>
    public class EmbeddedSpellChecker : ISpellChecker
    {
        static EmbeddedSpellChecker()
        {
            EmbeddedDllDependency.Init();
        }

        private static WordList Load(string languageCode)
        {
            const string resourceNamespaceBase = "";
            string languageResourceName = resourceNamespaceBase + languageCode;
            string affName = languageResourceName + ".aff.gz";
            string dicName = languageResourceName + ".dic.gz";

            Assembly assembly = typeof(EmbeddedSpellChecker).GetTypeInfo().Assembly;

            using Stream affCompressedStream =
                assembly.GetManifestResourceStream(affName) ?? throw new InvalidOperationException();
            using var affStream = new GZipStream(affCompressedStream, CompressionMode.Decompress);
            using Stream dicCompressedStream =
                assembly.GetManifestResourceStream(dicName) ?? throw new InvalidOperationException();
            using var dicStream = new GZipStream(dicCompressedStream, CompressionMode.Decompress);
            return WordList.CreateFromStreams(dicStream, affStream);
        }

        public EmbeddedSpellChecker(IEnumerable<string> languageCodes)
        {
            // LanguageCode = languageCode ?? throw new ArgumentNullException(nameof(languageCode));
            foreach (var languageCode in languageCodes)
            {
                WordLists.Add(Load(languageCode));
            }
        }

        protected List<WordList> WordLists { get; } = new();

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
