using FileKEY;
using System.Text;

namespace TestProject
{
    public class LanguageUnitTests
    {
        [Fact]
        public void LanguageInitializationEnglish()
        {
            Language.Initialize("en");
            var message = Language.GetMessage(Language.MessageKey.End);
            Assert.Equal("*END*", message);
        }

        [Fact]
        public void LanguageInitializationChinese()
        {
            Language.Initialize("zh");
            var message = Language.GetMessage(Language.MessageKey.End);
            Assert.Equal("*结束*", message);
        }

        [Fact]
        public void LanguageInitializationSystem()
        {
            Language.Initialize();
            var message = Language.GetMessage(Language.MessageKey.End);

            var currentCulture = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            if (currentCulture.Equals("en"))
            {
                Assert.Equal("*END*", message);
            }
            else if (currentCulture.Equals("zh"))
            {
                Assert.Equal("*结束*", message);
            }
            else
            {
                Assert.Equal("*END*", message);
            }
        }

    }
}
