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

        public WrongWordChecker(params string[] wrongWords)
            : this((IEnumerable<string>)wrongWords) { }

        public bool Check(string word) => !wrongWords.Contains(word);

        public IEnumerable<string> Suggest(string word)
            => Check(word) ? new[] { word } : Array.Empty<string>();
    }
}
