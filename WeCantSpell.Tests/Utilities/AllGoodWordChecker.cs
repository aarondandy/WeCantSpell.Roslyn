using System.Collections.Generic;

namespace WeCantSpell.Tests.Utilities
{
    public class AllGoodWordChecker : ISpellChecker
    {
        public bool Check(string word)
        {
            return true;
        }

        public IEnumerable<string> Suggest(string word)
        {
            return new[] { word };
        }
    }
}
