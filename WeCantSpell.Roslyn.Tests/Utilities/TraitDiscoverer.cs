using System.Collections.Generic;
using JetBrains.Annotations;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace WeCantSpell.Roslyn.Tests.Utilities
{
    [UsedImplicitly]
    public class TraitDiscoverer : ITraitDiscoverer
    {
        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        {
            var categoryName = traitAttribute.GetNamedArgument<string>("Name");
            yield return new KeyValuePair<string, string>("Category", categoryName);
        }
    }
}
