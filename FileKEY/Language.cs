
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
                        LoadLanguage();
                    break;
            }

            if (Enum.GetValues<MessageKey>().Length != messages.Count)
            {
                throw new Exception($"Language initialization error: The message count does not match. There should be {Enum.GetValues<MessageKey>().Length} records, but there are actually only {messages.Count}.");
            }
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
            Miss,
            NoKeyTheLengthIs,
            NoKeyInFile,
            ParameterLanguageUsageErrorMissingLanguageCode
        }

        private static void Initialize_en()
        {
            messages.Clear();
            messages.Add(MessageKey.End, "*END*");
            messages.Add(MessageKey.DisplayCompletedPressEnterToContinue, "Display completed, press enter to continue.");
            messages.Add(MessageKey.UnrecognizedParameters, "Unrecognized parameters:{0}");
            messages.Add(MessageKey.TooManyParameters, "Too many parameters:{0}");
            messages.Add(MessageKey.PleaseEnterTheFilePath, "Please enter the file path:");
            messages.Add(MessageKey.PleaseEnterTheFilePathUseDefaultInputPleaseEnterDirectly, "Please enter the file path, Use default input ({0}), Please enter directly:");
            messages.Add(MessageKey.TheInputFilePathDoesNotExist, "The input file path {0} does not exist");
            messages.Add(MessageKey.PleaseEnterTheVerificationKeyValueOrTheFilePathWhereTheKeyValueIsStored, "Please enter the verification key value or the file path where the key value is stored:");
            messages.Add(MessageKey.Wait, "Wait >>");
            messages.Add(MessageKey.ThereIsNoSuchFile, "There is no such file!");
            messages.Add(MessageKey.ProcessCompleted, " Process completed");
            messages.Add(MessageKey.Matched, "matched-{0}-{1}");
            messages.Add(MessageKey.Miss, "miss-{0}");

            messages.Add(MessageKey.NoKeyTheLengthIs, "NoKeyTheLengthIs({0})");
            messages.Add(MessageKey.NoKeyInFile, "NoKeyInFile({0})");

            messages.Add(MessageKey.ParameterLanguageUsageErrorMissingLanguageCode, "Parameter - Language usage error, missing language code.");

        }

        private static void Initialize_zh()
        {
            messages.Clear();
            messages.Add(MessageKey.End, "*结束*");
            messages.Add(MessageKey.DisplayCompletedPressEnterToContinue, "显示完毕，按回车继续下一个。");
            messages.Add(MessageKey.UnrecognizedParameters, "无法识别的参数：{0}");
            messages.Add(MessageKey.TooManyParameters, "参数过多：{0}");
            messages.Add(MessageKey.PleaseEnterTheFilePath, "请输入文件路径：");
            messages.Add(MessageKey.PleaseEnterTheFilePathUseDefaultInputPleaseEnterDirectly, "请输入文件路径，使用默认输入（{0}），请直接回车：");
            messages.Add(MessageKey.TheInputFilePathDoesNotExist, "输入的文件路径{0}不存在");
            messages.Add(MessageKey.PleaseEnterTheVerificationKeyValueOrTheFilePathWhereTheKeyValueIsStored, "请输入验证Key值或存储Key值的文件路径：");
            messages.Add(MessageKey.Wait, "稍等 >>");
            messages.Add(MessageKey.ThereIsNoSuchFile, "没有这个文件！");
            messages.Add(MessageKey.ProcessCompleted, " 处理完毕");
            messages.Add(MessageKey.Matched, "已匹配-{0}-{1}");
            messages.Add(MessageKey.Miss, "不相同-{0}");

            messages.Add(MessageKey.NoKeyTheLengthIs, "没有Hash的长度是（{0}）");
            messages.Add(MessageKey.NoKeyInFile, "没有Hash在文件中被匹配（{0}）");

            messages.Add(MessageKey.ParameterLanguageUsageErrorMissingLanguageCode, "参数-Language使用错误，缺少语言代码。");

        }

        public static string GetMessage(MessageKey message, params Object[]? formatArgs)
        {
            var msg = messages[message];

            if (formatArgs is not null)
                msg = string.Format(msg, formatArgs);

            return msg;
        }

        private static void LoadLanguage()
        {
            if (File.Exists("language.txt"))
            {
                messages.Clear();
                using (StreamReader reader = new StreamReader("language.txt"))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var item = line.Split('=');
                        if (item.Length == 2)
                        {
                            messages.Add(Enum.Parse<MessageKey>(item[0]), item[1]);
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
                throw new Exception("Could not find the corresponding language.txt file for the language. Please create your own using the on-screen text style.");
            }
        }
    }
}
