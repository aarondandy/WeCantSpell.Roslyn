using System;
using System.IO;
using System.Linq;
using WeCantSpell.Hunspell;
using WeCantSpell.Roslyn.Config;
using WeCantSpell.Roslyn.Infrastructure;

namespace WeCantSpell.Roslyn
{
    internal sealed class ConfigurableSpellChecker : EmbeddedSpellChecker
    {
        private readonly IFileSystem _fileSystem;

        private WordList LoadFromFile(string path)
        {
            var affixFilePath = FindAffixFilePath(path);
            using var dictionaryStream =
                _fileSystem.ReadStream(path)
                ?? throw new InvalidOperationException($"File not found {path}");
            using var affixStream = ReadStreamOrDefault(affixFilePath);
            return WordList.CreateFromStreams(dictionaryStream, affixStream);
        }

        private Stream ReadStreamOrDefault(string affixFilePath)
        {
            return _fileSystem.FileExists(affixFilePath)
                ? _fileSystem.ReadStream(affixFilePath)
                : new NullStream();
        }

        public ConfigurableSpellChecker(SpellCheckerOptions options)
            : base(options.LanguageCodes)
        {
            _fileSystem = options.FileSystem;
            foreach (var path in options.AdditionalDictionaryPaths)
            {
                WordLists.Add(LoadFromFile(path));
            }
        }

        private string FindAffixFilePath(string? dictionaryFilePath)
        {
            string? path =
                dictionaryFilePath != null
                    ? _fileSystem.GetDirectoryName(dictionaryFilePath)
                    : throw new ArgumentNullException(nameof(dictionaryFilePath));
            if (string.IsNullOrEmpty(path))
            {
                return _fileSystem.ChangeExtension(dictionaryFilePath, "aff");
            }

            string? affixFilePath = _fileSystem
                .GetFiles(
                    path!,
                    Path.GetFileNameWithoutExtension(dictionaryFilePath) + ".*",
                    SearchOption.TopDirectoryOnly
                )
                .FirstOrDefault(
                    affFilePath =>
                        ".AFF".Equals(
                            Path.GetExtension(affFilePath),
                            StringComparison.OrdinalIgnoreCase
                        )
                );
            return affixFilePath ?? Path.ChangeExtension(dictionaryFilePath, "aff");
        }
    }
}
