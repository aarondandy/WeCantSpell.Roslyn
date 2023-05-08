using System.Collections.Generic;
using JetBrains.Annotations;

namespace WeCantSpell.Roslyn.Config
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record ConfigFile
    {
        public bool IsRoot { get; set; }
        public List<string>? Languages { get; set; }
    }
}
