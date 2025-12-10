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

        [Fact]
        public void LanguageCreateLanguageFileAndLoadDefault()
        {
            var message = new StringBuilder();

            if (File.Exists("language.txt"))
                File.Delete("language.txt");

            Language.Initialize("en");

            foreach (Language.MessageKey msg in Enum.GetValues(typeof(Language.MessageKey)))
            {
                message.AppendLine($"{msg}={Language.GetMessage(msg)}");
            }
            Assert.True(message.Length > 0);

            Assert.Contains("*END*", message.ToString());

            message = message.Replace("End=*END*", "End=*end*");
            File.WriteAllText("language.txt", message.ToString());
            Language.Initialize("fr");

            Assert.Equal("*end*", Language.GetMessage(Language.MessageKey.End));

            if (File.Exists("language.txt"))
                File.Delete("language.txt");

        }

    }
}
