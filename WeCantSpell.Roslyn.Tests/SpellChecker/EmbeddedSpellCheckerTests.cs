using FluentAssertions.Execution;
using WeCantSpell.Roslyn.Tests.Utilities;

namespace WeCantSpell.Roslyn.Tests.SpellChecker
{
    [TestCategory("SpellChecker")]
    public class EmbeddedSpellCheckerTests
    {
        [Fact]
        public void ShouldReadFromEnglishDictionary()
        {
            var checker = new EmbeddedSpellChecker(new[] { "en-US" });
            using (new AssertionScope())
            {
                checker.Check("Thas").Should().Be(false);
                checker.Check("This").Should().Be(true);
            }
        }

        [Fact]
        public void ShouldReadFromTwoDictionaries()
        {
            var checker = new EmbeddedSpellChecker(new[] { "en-US", "ru-RU" });
            using (new AssertionScope())
            {
                checker.Check("Thas").Should().Be(false);
                checker.Check("Этат").Should().Be(false);
                checker.Check("This").Should().Be(true);
                checker.Check("Тот").Should().Be(true);
            }
        }

        [Fact]
        public void ShouldProvideSuggestions()
        {
            ISpellChecker checker = new EmbeddedSpellChecker(new[] { "en-US", "ru-RU" });
            using (new AssertionScope())
            {
                checker.Suggest("Thes").Should().Contain("The");
                checker.Suggest("Этат").Should().Contain("Этот");
            }
        }
    }
}
