using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using JetBrains.Annotations;
using WeCantSpell.Roslyn.Infrastructure;

namespace WeCantSpell.Roslyn.Config
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public sealed class SpellCheckerOptions
    {
        private static string[] DefaultLanguageCodes { get; set; } = { "en-US", "ru-RU" };

        public static string[] ConfigFileNames { get; } = { ".wecantspell", ".wecantspell.json" };

        public static string[] DictionaryFileNames { get; } = { ".spelling.dic", ".directory.dic", ".wecantspell.dic" };
        public IList<string> LanguageCodes { get; init; } = DefaultLanguageCodes;

        public IList<string> AdditionalDictionaryPaths { get; init; } = new List<string>();

        internal IFileSystem FileSystem { get; }

        public SpellCheckerOptions(IFileSystem fileSystem, string path)
            : this(fileSystem)
        {
            ReadFromDirectory(path);
        }

        public SpellCheckerOptions(string path)
            : this()
        {
            ReadFromDirectory(path);
        }

        public SpellCheckerOptions()
            : this(new FileSystem()) { }

        public SpellCheckerOptions(IFileSystem fileSystem)
        {
            FileSystem = fileSystem;
        }

        private void ReadFromDirectory(string path)
        {
            var directory = FileSystem.DirectoryExists(path) ? path : FileSystem.GetDirectoryName(path);

            while (directory != null)
            {
                var fileName = GuessConfigFileName(directory);
                if (fileName != null)
                {
                    ConfigFile? configFile = Parse(fileName);
                    MergeFrom(configFile);
                    if (configFile?.IsRoot == true)
                        break;
                }

                var dictionaryName = GuessDictionaryFileName(directory);
                if (dictionaryName != null)
                {
                    AdditionalDictionaryPaths.Add(dictionaryName);
                }

                directory = FileSystem.GetParentDirectory(directory);
            }
        }

        private ConfigFile? Parse(string filePath)
        {
            string json = FileSystem.ReadAllText(filePath);
            var config = JsonSerializer.Deserialize<ConfigFile>(json);
            return config;
        }

        private void MergeFrom(ConfigFile? localOptions)
        {
            if (localOptions?.Languages == null)
                return;
            foreach (var code in localOptions.Languages.Where(code => !LanguageCodes.Contains(code)))
            {
                LanguageCodes.Add(code);
            }
        }

        private string? GuessConfigFileName(string directoryPath)
        {
            return ConfigFileNames
                .Select(fileName => TryFileExists(directoryPath, fileName))
                .FirstOrDefault(fullPath => fullPath != null);
        }

        private string? GuessDictionaryFileName(string directoryPath)
        {
            return DictionaryFileNames
                .Select(fileName => TryFileExists(directoryPath, fileName))
                .FirstOrDefault(fullPath => fullPath != null);
        }

        private string? TryFileExists(string directoryName, string fileName)
        {
            var tryFilePath = FileSystem.CombinePath(directoryName, fileName);
            return FileSystem.FileExists(tryFilePath) ? tryFilePath : null;
        }

        public string? FindDictionaryForPath(string? path)
        {
            if (path == null)
                return null;
            var directory = FileSystem.DirectoryExists(path) ? path : FileSystem.GetDirectoryName(path);

            while (directory != null)
            {
                var dictionaryName = GuessDictionaryFileName(directory);
                if (dictionaryName != null)
                {
                    return dictionaryName;
                }

                directory = FileSystem.GetParentDirectory(directory);
            }

            return null;
        }
    }
}
