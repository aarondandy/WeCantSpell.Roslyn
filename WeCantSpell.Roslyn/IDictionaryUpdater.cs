using System.Threading.Tasks;

namespace WeCantSpell.Roslyn
{
    public interface IDictionaryUpdater
    {
        Task AddToLocalDictionaryAsync(string dictionaryWord);
    }
}
