using System;
using System.Collections.Generic;

namespace WeCantSpell.Roslyn.Infrastructure
{
    public class DebugTestingSpellChecker : ISpellChecker
    {
        readonly HashSet<string> badWords = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase)
        {
            "nope"
        };

        public bool Check(string word) =>
            word == null || !badWords.Contains(word);

        public IEnumerable<string> Suggest(string word) =>
            new[] { Check(word) ? (word ?? string.Empty) : "nah" };
    }
}
