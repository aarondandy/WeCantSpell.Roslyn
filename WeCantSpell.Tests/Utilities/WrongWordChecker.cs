using System;
using System.Collections.Generic;

namespace WeCantSpell.Tests.Utilities
{
    public class WrongWordChecker : ISpellChecker
    {
        private HashSet<string> wrongWords;

        public WrongWordChecker(string wrongWord)
        {
            this.wrongWords = new HashSet<string>
            {
                wrongWord
            };
        }

        public WrongWordChecker(IEnumerable<string> wrongWords)
        {
            this.wrongWords = new HashSet<string>(wrongWords);
        }

        public bool Check(string word)
        {
            return !wrongWords.Contains(word);
        }

        public IEnumerable<string> Suggest(string word)
        {
            return Check(word)
                ? new[] { word }
                : Array.Empty<string>();
        }
    }
}
