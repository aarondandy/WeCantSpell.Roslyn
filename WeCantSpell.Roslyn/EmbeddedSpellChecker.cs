using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Reflection;
using WeCantSpell.Hunspell;

namespace WeCantSpell.Roslyn
{
    public class EmbeddedSpellChecker : ISpellChecker
    {
        static WordList Load(string languageCode)
        {
            const string resourceNamespaceBase = "WeCantSpell.Roslyn.DefaultDictionaries.";
            var languageResourceName = resourceNamespaceBase + languageCode;
            var affName = languageResourceName + ".aff.compressed";
            var dicName = languageResourceName + ".dic.compressed";

            var assembly = typeof(EmbeddedSpellChecker).GetTypeInfo().Assembly;

            using (var affCompressedStream = assembly.GetManifestResourceStream(affName))
            using (var affStream = new DeflateStream(affCompressedStream, CompressionMode.Decompress))
            using (var dicCompressedStream = assembly.GetManifestResourceStream(dicName))
            using (var dicStream = new DeflateStream(dicCompressedStream, CompressionMode.Decompress))
            {
                return WordList.CreateFromStreams(dicStream, affStream);
            }
        }

        public EmbeddedSpellChecker(string languageCode)
        {
            LanguageCode = languageCode ?? throw new ArgumentNullException(nameof(languageCode));
            WordList = Load(languageCode);
        }

        public string LanguageCode { get; }

        WordList WordList { get; }

        public bool Check(string word) =>
            WordList.Check(word);

        public IEnumerable<string> Suggest(string word) =>
            WordList.Suggest(word);
    }
}
