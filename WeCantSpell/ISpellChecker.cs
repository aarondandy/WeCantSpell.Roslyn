using System.Collections.Generic;

namespace WeCantSpell
{
    public interface ISpellChecker
    {
        bool Check(string word);

        IEnumerable<string> Suggest(string word);
    }
}
