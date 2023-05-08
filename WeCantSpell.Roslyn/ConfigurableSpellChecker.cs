using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WeCantSpell.Hunspell;
using WeCantSpell.Roslyn.Config;
using WeCantSpell.Roslyn.Infrastructure;

namespace WeCantSpell.Roslyn
{
    [PublicAPI]
    public sealed class ConfigurableSpellChecker : EmbeddedSpellChecker, IDictionaryUpdater
    {
        private readonly SpellCheckerOptions _options;
        private Dictionary<string, WordList> _wordLists = new();
        private string? _updateableProjectWordListPath;

        public ConfigurableSpellChecker(SpellCheckerOptions options)
            : base(options.LanguageCodes)
        {
            _options = options;
            foreach (var path in options.AdditionalDictionaryPaths)
            {
                var wordList = LoadFromFile(path);
                _wordLists[path] = wordList;
                WordLists.Add(wordList);
            }
            // All directories are supposed to be nested, so the longest path is closest to context
            _updateableProjectWordListPath = _wordLists.Keys.OrderByDescending(k => k.Length).FirstOrDefault();
        }

        private WordList LoadFromFile(string path)
        {
            var affixFilePath = FindAffixFilePath(path);
            using Stream dictionaryStream =
                _options.FileSystem.ReadStream(path) ?? throw new InvalidOperationException($"File not found {path}");
            using Stream affixStream = ReadStreamOrDefault(affixFilePath);
            return WordList.CreateFromStreams(dictionaryStream, affixStream);
        }

        private Stream ReadStreamOrDefault(string affixFilePath)
        {
            return _options.FileSystem.FileExists(affixFilePath)
                ? _options.FileSystem.ReadStream(affixFilePath)
                : new NullStream();
        }

        private string FindAffixFilePath(string? dictionaryFilePath)
        {
            var path =
                dictionaryFilePath != null
                    ? _options.FileSystem.GetDirectoryName(dictionaryFilePath)
                    : throw new ArgumentNullException(nameof(dictionaryFilePath));
            if (string.IsNullOrEmpty(path))
            {
                return _options.FileSystem.ChangeExtension(dictionaryFilePath, "aff");
            }

            var affixFilePath = _options.FileSystem
                .GetFiles(
                    path!,
                    Path.GetFileNameWithoutExtension(dictionaryFilePath) + ".*",
                    SearchOption.TopDirectoryOnly
                )
                .FirstOrDefault(
                    affFilePath => ".AFF".Equals(Path.GetExtension(affFilePath), StringComparison.OrdinalIgnoreCase)
                );
            return affixFilePath ?? Path.ChangeExtension(dictionaryFilePath, "aff");
        }

        public async Task AddToLocalDictionaryAsync(string dictionaryWord)
        {
            if (_updateableProjectWordListPath == null)
                throw new InvalidOperationException(
                    "No updateable project dictionaries are defined. Please create a file .directory.dic inside your project and reload analyzer"
                );
            using (StreamWriter writer = _options.FileSystem.AppendText(_updateableProjectWordListPath))
            {
                await writer.WriteLineAsync(dictionaryWord);
            }
            WordLists.Remove(_wordLists[_updateableProjectWordListPath]);
            var newWordList = LoadFromFile(_updateableProjectWordListPath);
            WordLists.Add(newWordList);
            _wordLists[_updateableProjectWordListPath] = newWordList;
        }
    }
}
