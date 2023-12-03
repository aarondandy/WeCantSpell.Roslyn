using JetBrains.Annotations;
using Xunit.Sdk;

namespace WeCantSpell.Roslyn.Tests.Utilities
{
    [AttributeUsage(AttributeTargets.Class)]
    [TraitDiscoverer("WeCantSpell.Roslyn.Tests.Utilities.TraitDiscoverer", "WeCantSpell.Roslyn.Tests")]
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class TestCategoryAttribute : Attribute, ITraitAttribute
    {
        public string Name { get; }

        public TestCategoryAttribute(string categoryName)
        {
            Name = categoryName;
        }
    }
    
}
