using System.Collections.Generic;
using JetBrains.Annotations;

namespace WeCantSpell.Roslyn.Config
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record ConfigFile
    {
        public bool IsRoot { get; }
        public IEnumerable<string>? Languages { get; }
    }
}
