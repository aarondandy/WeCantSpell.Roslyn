using System.Collections.Generic;
using System.Linq;

namespace WeCantSpell.Roslyn.Tests.Utilities
{
    public class WrongWordChecker : ISpellChecker
    {
        private readonly HashSet<string> _wrongWords;

        public WrongWordChecker(string wrongWord) => _wrongWords = new HashSet<string> { wrongWord };

        private WrongWordChecker(IEnumerable<string> wrongWords) => _wrongWords = new HashSet<string>(wrongWords);

        public WrongWordChecker(params string[] wrongWords)
            : this((IEnumerable<string>)wrongWords) { }

        public bool Check(string word) => !_wrongWords.Contains(word);

        public IEnumerable<string> Suggest(string word) => Check(word) ? new[] { word } : Enumerable.Empty<string>();
    }
}
