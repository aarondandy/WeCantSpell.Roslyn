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
        private const string AdditionalDictionariesFolderName = ".WeCantSpellUserDictionaries";
        private const string CustomDictFileName = "WeCantSpellUserDictionary";

        public EmbeddedSpellChecker(string languageCode)
        {
            LanguageCode = languageCode ?? throw new ArgumentNullException(nameof(languageCode));
            var defaultWordList = Load(languageCode);
            WordLists.Add(defaultWordList);
            string[] customDictWords = LoadUserWordsDictionary();
            WordLists.Add(WordList.CreateFromWords(customDictWords));
            WordLists.AddRange(LoadFromFolder());
        }

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
        static List<WordList> LoadFromFolder()
        {
            var wordLists = new List<WordList>();
            string dictionariesFolder = GetDictionariesFolder();
            var dictionariesDirectory = Directory.CreateDirectory(dictionariesFolder);
            var existingFiles = dictionariesDirectory.GetFiles();
            var dicFiles = existingFiles.Where(f => f.Extension == ".dic").ToList();

            foreach (var dicFile in dicFiles)
            {
                var affFileName = dicFile.FullName.Replace(".dic", ".aff");
                if (existingFiles.SingleOrDefault(f => f.FullName == affFileName) != null)
                {
                   var wordList = WordList.CreateFromFiles(dicFile.FullName, affFileName);
                    wordLists.Add(wordList);
                }
            }
            return wordLists;
        }

        private static string[] LoadUserWordsDictionary()
        {
            string dictionariesFolder = GetDictionariesFolder();
            var customDictionaryPath = Path.Combine(dictionariesFolder, $"{CustomDictFileName}.dic");
            Directory.CreateDirectory(dictionariesFolder);
            if (!File.Exists(customDictionaryPath))
            {
                File.Create(customDictionaryPath).Dispose();
            }
            var customDictWords = File.ReadAllLines(customDictionaryPath);
            return customDictWords;
        }

        private static string GetDictionariesFolder()
        {
            string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var dictionariesFolder = Path.Combine(userFolder, AdditionalDictionariesFolderName);
            return dictionariesFolder;
        }

        public string LanguageCode { get; }

        List<WordList> WordLists { get; } = new List<WordList>();

        public bool Check(string word) =>
            WordLists.Any(w => w.Check(word));

        public IEnumerable<string> Suggest(string word)
        {
            var suggestions = new List<string>();
            foreach (var wordList in WordLists)
            {
                suggestions.AddRange(wordList.Suggest(word));
            }
            return suggestions;
        }
    }
}
