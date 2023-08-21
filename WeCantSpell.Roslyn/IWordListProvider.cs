using WeCantSpell.Hunspell;

namespace WeCantSpell.Roslyn
{
    /// <summary>
    /// Generic interface for spell checker word list providers
    /// </summary>
    public interface IWordListProvider
    {
        WordList WordList { get; }
    }
}
