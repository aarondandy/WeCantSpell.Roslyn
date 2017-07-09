using System;
using System.Collections.Generic;

namespace WeCantSpell.Roslyn.Utilities
{
    public class DebugTestingSpellChecker : ISpellChecker
    {
        readonly HashSet<string> badWords = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase)
        {
            "nope"
        };

        public bool Check(string word)
        {
            if (word == null)
            {
                return true;
            }

            return !badWords.Contains(word);
        }

        public IEnumerable<string> Suggest(string word)
        {
            if (word == "nope")
            {
                return new[] { "nah" };
            }

            return new[] { word };
        }
    }
}
