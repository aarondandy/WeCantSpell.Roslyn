using System.Collections.Generic;
using System.Linq;
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

        public EmbeddedSpellChecker(IEnumerable<string> languageCodes)
        {
            // LanguageCode = languageCode ?? throw new ArgumentNullException(nameof(languageCode));
            foreach (var languageCode in languageCodes)
            {
                Providers.Add(EmbeddedWordListProvider.Load(languageCode));
            }
        }

        protected List<IWordListProvider> Providers { get; } = new();

        public bool Check(string word)
        {
            return Providers.Any(provider => provider.WordList.Check(word));
        }

        public IEnumerable<string> Suggest(string word)
        {
            return Providers.SelectMany(provider => provider.WordList.Suggest(word));
        }
    }
}
