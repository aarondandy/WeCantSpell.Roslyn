using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public abstract class CSharpParsingTestBase : CSharpTestBase
    {
        protected override async Task<TextAndVersion> ReadCodeFileAsSTextAndVersionAsync(string embeddedResourceFileName) =>
            TextAndVersion.Create(
                 SourceText.From(await ReadCodeFileAsStringAsync("Parsing.Files." + embeddedResourceFileName)),
                 VersionStamp.Default,
                 embeddedResourceFileName);
    }
}
