using System.Collections.Generic;

namespace WeCantSpell.Roslyn.Tests.Utilities
{
    public class AllGoodWordChecker : ISpellChecker
    {
        public bool Check(string word) => true;

        public IEnumerable<string> Suggest(string word) => new[] { word };
    }
}
