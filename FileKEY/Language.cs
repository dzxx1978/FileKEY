
using System.Globalization;

namespace FileKEY
{
    public static class Language
    {
        private static Dictionary<MessageKey, string> messages = new();

        public static void Initialize(string language = "")
        {
            var ifDefault = false;
            if (string.IsNullOrEmpty(language))
            {
                ifDefault = true;
                language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            }

            switch (language)
            {
                case "en":
                    Initialize_en();
                    break;
                case "zh":
                    Initialize_zh();
                    break;
                default:
                    if (ifDefault)
                        Initialize_en();
                    else
                        LoadLanguage(language);
                    break;
            }
        }

        static Language()
        {
            foreach (Language.MessageKey msg in Enum.GetValues(typeof(Language.MessageKey)))
            {
                messages.Add(msg, "");
            }

            Initialize();
        }

        public enum MessageKey
        {
            End,
            DisplayCompletedPressEnterToContinue,
            UnrecognizedParameters,
            TooManyParameters,
            PleaseEnterTheFilePath,
            PleaseEnterTheFilePathUseDefaultInputPleaseEnterDirectly,
            TheInputFilePathDoesNotExist,
            PleaseEnterTheVerificationKeyValueOrTheFilePathWhereTheKeyValueIsStored,
            Wait,
            ThereIsNoSuchFile,
            ProcessCompleted,
            Matched,
            MatchedInKeysFile,
            Miss,
            NoKeyTheLengthIs,
            NoKeyInFile,
            ParameterLanguageUsageErrorMissingLanguageCode
        }

        private static void Initialize_en()
        {

            messages[MessageKey.End] = "*END*";
            messages[MessageKey.DisplayCompletedPressEnterToContinue] = "Display completed, press enter to continue.";
            messages[MessageKey.UnrecognizedParameters] = "Unrecognized parameters:{0}";
            messages[MessageKey.TooManyParameters]="Too many parameters:{0}";
            messages[MessageKey.PleaseEnterTheFilePath]="Please enter the file path:";
            messages[MessageKey.PleaseEnterTheFilePathUseDefaultInputPleaseEnterDirectly]="Please enter the file path, Use default input ({0}), Please enter directly:";
            messages[MessageKey.TheInputFilePathDoesNotExist]="The input file path {0} does not exist";
            messages[MessageKey.PleaseEnterTheVerificationKeyValueOrTheFilePathWhereTheKeyValueIsStored]="Please enter the verification key value or the file path where the key value is stored:";
            messages[MessageKey.Wait]="Wait >>";
            messages[MessageKey.ThereIsNoSuchFile]="There is no such file!";
            messages[MessageKey.ProcessCompleted]=" Process completed";
            messages[MessageKey.Matched]="matched-{0}-{1}";
            messages[MessageKey.MatchedInKeysFile]="matched-{0}-{1}-{2}.{3}";
            messages[MessageKey.Miss]="miss-{0}";

            messages[MessageKey.NoKeyTheLengthIs]="NoKeyTheLengthIs({0})";
            messages[MessageKey.NoKeyInFile]="NoKeyInFile({0})";

            messages[MessageKey.ParameterLanguageUsageErrorMissingLanguageCode]="Parameter - Language usage error, missing language code.";

        }

        private static void Initialize_zh()
        {

            messages[MessageKey.End]="*结束*";
            messages[MessageKey.DisplayCompletedPressEnterToContinue]="显示完毕，按回车继续下一个。";
            messages[MessageKey.UnrecognizedParameters]="无法识别的参数：{0}";
            messages[MessageKey.TooManyParameters]="参数过多：{0}";
            messages[MessageKey.PleaseEnterTheFilePath]="请输入文件路径：";
            messages[MessageKey.PleaseEnterTheFilePathUseDefaultInputPleaseEnterDirectly]="请输入文件路径，使用默认输入（{0}），请直接回车：";
            messages[MessageKey.TheInputFilePathDoesNotExist]="输入的文件路径{0}不存在";
            messages[MessageKey.PleaseEnterTheVerificationKeyValueOrTheFilePathWhereTheKeyValueIsStored]="请输入验证Key值或存储Key值的文件路径：";
            messages[MessageKey.Wait]="稍等 >>";
            messages[MessageKey.ThereIsNoSuchFile]="没有这个文件！";
            messages[MessageKey.ProcessCompleted]=" 处理完毕";
            messages[MessageKey.Matched]="已匹配-{0}-{1}";
            messages[MessageKey.MatchedInKeysFile]="已匹配-{0}-{1}-{2}.{3}";
            messages[MessageKey.Miss]="不相同-{0}";

            messages[MessageKey.NoKeyTheLengthIs]="没有Hash的长度是（{0}）";
            messages[MessageKey.NoKeyInFile]="没有Hash在文件中被匹配（{0}）";

            messages[MessageKey.ParameterLanguageUsageErrorMissingLanguageCode]="参数-Language使用错误，缺少语言代码。";

        }

        public static string GetMessage(MessageKey message, params Object[]? formatArgs)
        {
            var msg = messages[message];

            if (formatArgs is not null && formatArgs.Length > 0)
                msg = string.Format(msg, formatArgs);

            return msg;
        }

        public static string GetHelpShown()
        {
            return @$"
https://github.com/dzxx1978/FileKEY
FileKEY {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()} by zxx 2025
FileKEY [path] [key] [-0tcms]
 -0 small print
 -t only type
 -c only crc
 -m only md5
 -s only sha256
";
        }

        private static void LoadLanguage(string language)
        {
            if (File.Exists($"language_{language}.txt"))
            {
                messages.Clear();
                using (StreamReader reader = new StreamReader($"language_{language}.txt"))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var item = line.Split('=');
                        if (item.Length == 2)
                        {
                            messages[Enum.Parse<MessageKey>(item[0])] = item[1];
                        }
                    }
                }
            }
            else
            {
                Initialize_en();
                foreach (var msg in messages)
                {
                    Message.WriteLine($"{msg.Key}={msg.Value}");
                }
                throw new Exception($"Could not find the corresponding language_{language}.txt file for the language. Please create your own using the on-screen text style.");
            }
        }
    }
}
