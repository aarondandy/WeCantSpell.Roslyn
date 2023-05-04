using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using WeCantSpell.Roslyn.Config;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.SpellChecker
{
    public class ConfigurableSpellCheckerTests
    {
        [Fact]
        public void ShouldReadDictionaryFromFileSystem()
        {
            var spellchecker = new ConfigurableSpellChecker(new SpellCheckerOptions
                {
                    LanguageCodes = new List<string>(),
                    AdditionalDictionaryPaths = new List<string>
                    {
                        Path.Combine(Directory.GetCurrentDirectory(), "SpellChecker", "Files", "FantasyWords.dic")
                    }
                }
            );
            spellchecker.Check("Bazinga").Should().BeTrue();
            spellchecker.Check("Froomplestoot").Should().BeFalse();
        }
        
        [Fact]
        public void ShouldReadDictionaryFromResourceFileSystem()
        {
            var fileSystem = new ResourceFileSystem();
            var spellchecker = new ConfigurableSpellChecker(new SpellCheckerOptions(fileSystem, "SpellChecker.Files")
                {
                    LanguageCodes = new List<string>(),
                    AdditionalDictionaryPaths = new List<string>
                    {
                        "Files.FantasyWords.dic"
                    }
                }
            );
            spellchecker.Check("Bazinga").Should().BeTrue();
            spellchecker.Check("Froomplestoot").Should().BeFalse();
        }
    }
}
