namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.Parsing
{
    public abstract class CSharpParsingTestBase : CSharpTestBase
    {
        protected override string CreateResourceNameFromFileName(string fileName) =>
            "Parsing.Files." + fileName;
    }
}
