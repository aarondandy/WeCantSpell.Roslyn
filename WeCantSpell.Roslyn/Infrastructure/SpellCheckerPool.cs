using System.Collections.Concurrent;
using WeCantSpell.Roslyn.Config;

namespace WeCantSpell.Roslyn.Infrastructure
{
    public sealed class SpellCheckerPool
    {
        private readonly ConcurrentDictionary<SpellCheckerOptions, ConfigurableSpellChecker> _pool =
            new();
        private static SpellCheckerPool? s_shared;

        public static SpellCheckerPool Shared => s_shared ??= new SpellCheckerPool();

        internal ISpellChecker Get(SpellCheckerOptions options)
        {
            return _pool.GetOrAdd(
                options,
                spellCheckerOptions => new ConfigurableSpellChecker(spellCheckerOptions)
            );
        }
    }
}
