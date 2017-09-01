namespace WeCantSpell.Roslyn.Tests.Integration.CSharp.InheritedWords
{
    public abstract class CSharpInheritedWordsTestBase : CSharpTestBase
    {
        protected override string CreateResourceNameFromFileName(string fileName) =>
            "InheritedWords.Files." + fileName;
    }
}
