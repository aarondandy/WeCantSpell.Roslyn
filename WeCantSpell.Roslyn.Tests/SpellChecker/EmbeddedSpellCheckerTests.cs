using FluentAssertions;
using Xunit;

namespace WeCantSpell.Roslyn.Tests.SpellChecker
{
    public class EmbeddedSpellCheckerTests
    {
        [Fact]
        public void ShouldReadFromEnglishDictionary()
        {
            var checker = new EmbeddedSpellChecker(new[] { "en-US" });
            checker.Check("Thas").Should().Be(false);
            checker.Check("This").Should().Be(true);
        }

        [Fact]
        public void ShouldReadFromTwoDictionaries()
        {
            var checker = new EmbeddedSpellChecker(new[] { "en-US", "ru-RU" });
            checker.Check("Thas").Should().Be(false);
            checker.Check("Этат").Should().Be(false);
            checker.Check("This").Should().Be(true);
            checker.Check("Тот").Should().Be(true);
        }

        [Fact]
        public void ShouldProvideSuggestions()
        {
            ISpellChecker checker = new EmbeddedSpellChecker(new[] { "en-US", "ru-RU" });
            checker.Suggest("Thes").Should().Contain("The");
            checker.Suggest("Этат").Should().Contain("Этот");
        }
    }
}
