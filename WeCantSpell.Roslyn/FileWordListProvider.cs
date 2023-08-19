using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using WeCantSpell.Hunspell;
using WeCantSpell.Roslyn.Config;
using WeCantSpell.Roslyn.Infrastructure;

namespace WeCantSpell.Roslyn
{
    /// <summary>
    /// Word list provider for dictionaries loaded from files
    /// </summary>
    /// <remarks>
    /// Global cache of dictionaries is maintained and updated when the file is modified
    /// </remarks>
    public class FileWordListProvider : IWordListProvider
    {
        public string Path { get; }
        public WordList WordList
        {
            get { return GetOrUpdateWordList(); }
            set { _wordList = value; }
        }

        private WordList _wordList;
        private IFileSystem FileSystem { get; }

        // Local caching
        private DateTime _lastWriteTime;
        private DateTime _lastCheckTime;

        // Global caching
        private static ConcurrentDictionary<string, FileWordListProvider> s_instances = new();

        /// <summary>
        /// Constructor should not be called directly, use <see cref="Load"/> instead
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileSystem"></param>
        private FileWordListProvider(string path, IFileSystem fileSystem)
        {
            Path = path;
            FileSystem = fileSystem;
            _wordList = LoadFromFile();
        }

        /// <summary>
        /// Update from file
        /// </summary>
        public void Update()
        {
            _wordList = LoadFromFile();
        }

        /// <summary>
        /// Get from cache or load from file
        /// </summary>
        /// <param name="path">Path to the dictionary file</param>
        /// <param name="fileSystem">File system provider</param>
        /// <returns></returns>
        public static FileWordListProvider Load(string path, IFileSystem fileSystem)
        {
            return s_instances.GetOrAdd(
                $"{fileSystem.FileSystemType}:{path}",
                _ => new FileWordListProvider(path, fileSystem)
            );
        }

        private WordList GetOrUpdateWordList()
        {
            if (_lastCheckTime + TimeSpan.FromMilliseconds(100) > DateTime.UtcNow)
            {
                return _wordList;
            }
            _lastCheckTime = DateTime.UtcNow;
            var lastWriteTime = File.GetLastWriteTime(Path);
            if (lastWriteTime > _lastWriteTime)
            {
                _lastWriteTime = lastWriteTime;
                Update();
            }
            return _wordList;
        }

        private WordList LoadFromFile()
        {
            var affixFilePath = FindAffixFilePath(Path, FileSystem);
            using Stream dictionaryStream =
                FileSystem.ReadStream(Path) ?? throw new InvalidOperationException($"File not found {Path}");
            using Stream affixStream = ReadStreamOrDefault(affixFilePath, FileSystem);
            var wordList = WordList.CreateFromStreams(dictionaryStream, affixStream);
            return wordList;
        }

        private static Stream ReadStreamOrDefault(string affixFilePath, IFileSystem fileSystem)
        {
            return fileSystem.FileExists(affixFilePath) ? fileSystem.ReadStream(affixFilePath) : new NullStream();
        }

        public static string FindAffixFilePath(string? dictionaryFilePath, IFileSystem fileSystem)
        {
            var path =
                dictionaryFilePath != null
                    ? fileSystem.GetDirectoryName(dictionaryFilePath)
                    : throw new ArgumentNullException(nameof(dictionaryFilePath));
            if (string.IsNullOrEmpty(path))
            {
                return fileSystem.ChangeExtension(dictionaryFilePath, "aff");
            }

            var affixFilePath = fileSystem
                .GetFiles(
                    path!,
                    System.IO.Path.GetFileNameWithoutExtension(dictionaryFilePath) + ".*",
                    SearchOption.TopDirectoryOnly
                )
                .FirstOrDefault(
                    affFilePath =>
                        ".AFF".Equals(System.IO.Path.GetExtension(affFilePath), StringComparison.OrdinalIgnoreCase)
                );
            return affixFilePath ?? System.IO.Path.ChangeExtension(dictionaryFilePath, "aff");
        }
    }
}
