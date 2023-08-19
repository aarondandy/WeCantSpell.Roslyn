using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WeCantSpell.Hunspell;
using WeCantSpell.Roslyn.Config;

namespace WeCantSpell.Roslyn
{
    /// <summary>
    /// Spell checker with configuration and dictionaries on disk
    /// </summary>
    [PublicAPI]
    public sealed class ConfigurableSpellChecker : EmbeddedSpellChecker, IDictionaryUpdater
    {
        private readonly SpellCheckerOptions _options;
        private FileWordListProvider? _updateableProjectWordList;

        public ConfigurableSpellChecker(SpellCheckerOptions options)
            : base(options.LanguageCodes)
        {
            _options = options;
            // All directories are supposed to be nested, so the longest path is closest to context
            foreach (var path in options.AdditionalDictionaryPaths.OrderByDescending(k => k.Length))
            {
                var wordList = FileWordListProvider.Load(path, _options.FileSystem);
                // On the first pass this will be the longest path, so it will be the project dictionary
                _updateableProjectWordList ??= wordList;
                Providers.Add(wordList);
            }
        }

        public async Task AddToLocalDictionaryAsync(string dictionaryWord)
        {
            if (_updateableProjectWordList == null)
                throw new InvalidOperationException(
                    "No updateable project dictionaries are defined. Please create a file .directory.dic inside your project and reload analyzer"
                );
            using (StreamWriter writer = _options.FileSystem.AppendText(_updateableProjectWordList.Path))
            {
                await writer.WriteLineAsync(dictionaryWord);
            }
            _updateableProjectWordList?.Update();
        }
    }
}
