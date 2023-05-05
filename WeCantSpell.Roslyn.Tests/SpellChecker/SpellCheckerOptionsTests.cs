using System.IO;
using FluentAssertions;
using WeCantSpell.Roslyn.Config;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.SpellChecker
{
    public class SpellCheckerOptionsTests
    {
        [Fact]
        public void ShouldReadConfigurationFromDirectory()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "SpellChecker", "TestDirectory");
            var options = new SpellCheckerOptions(path);
            options.LanguageCodes.Should().HaveCount(2).And.ContainInOrder("en-US", "ru-RU");
            options.AdditionalDictionaryPaths
                .Should()
                .HaveCount(1)
                .And.SatisfyRespectively(name =>
                {
                    name.Should().EndWith(".directory.dic");
                    File.Exists(name).Should().BeTrue();
                });
        }
    }
}
