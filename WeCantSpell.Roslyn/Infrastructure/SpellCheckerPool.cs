using System.Collections.Concurrent;
using WeCantSpell.Roslyn.Config;

namespace WeCantSpell.Roslyn.Infrastructure
{
    /// <summary>
    /// The pool is keeping track of dictionaries, their locations
    /// </summary>
    public sealed class SpellCheckerPool
    {
        private static SpellCheckerPool? s_shared;

        private readonly ConcurrentDictionary<SpellCheckerOptions, ConfigurableSpellChecker> _pool = new();

        public static SpellCheckerPool Shared => s_shared ??= new SpellCheckerPool();

        internal ISpellChecker Get(SpellCheckerOptions options)
        {
            return InternalGet(options);
        }

        internal IDictionaryUpdater GetUpdater(SpellCheckerOptions options)
        {
            return InternalGet(options);
        }

        private ConfigurableSpellChecker InternalGet(SpellCheckerOptions options)
        {
            return _pool.GetOrAdd(options, spellCheckerOptions => new ConfigurableSpellChecker(spellCheckerOptions));
        }
    }
}
