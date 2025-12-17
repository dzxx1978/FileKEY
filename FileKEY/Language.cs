using System.Globalization;
using System.Text.RegularExpressions;

namespace FileKEY
{
    public static class Language
    {
        public static readonly string Type = "type";
        public static readonly string Crc = "CRC32";
        public static readonly string Md5 = "MD5";
        public static readonly string Sha256 = "SHA256";

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
                case "en-US":
                    Initialize_en();
                    break;
                case "zh":
                case "zh-CN":
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
            messagesClear();
            Initialize();
        }

        private static void messagesClear() {

            messages.Clear();
            foreach (MessageKey msg in Enum.GetValues(typeof(MessageKey)))
            {
                messages.Add(msg, "");
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
            PleaseEnterTheMinimumNumberOfGroupsToDisplay,
            PleaseEnterTheVerificationKeyValueOrTheFilePathWhereTheKeyValueIsStored,
            PleaseEnterTheNumberOfScannedSubdirectoriesAtDifferentLevels,
            Wait,
            ThereIsNoSuchFile,
            ProcessCompleted,
            Matched,
            MatchedInKeysFile,
            Miss,
            NoKeyTheLengthIs,
            NoKeyInFile,
            ParameterLanguageUsageErrorMissingLanguageCode,
            ParameterErrorMissingPath,
            ParameterError,
            MenuTitle,
            MenuSetPath,
            MenuSetKey,
            MenuSetOtherOptions,
            MenuConfigFiles,
            MenuShowOptions,
            MenuShowHelp,
            MenuRun,
            MenuClose,
            MenuReSet,
            MenuSelected,
            SaveCurrentOptions,
            PleaseEnterTheConfigurationFileName,
            Set,
            Del,
            None,
            Detailed,
            Small,
            Display,
            Hash,
            All,
            Language,
            SaveToFile,
        }

        private static void Initialize_en()
        {
            messagesClear();
            messages[MessageKey.End] = "*END*";
            messages[MessageKey.DisplayCompletedPressEnterToContinue] = "Display completed, press enter to continue.";
            messages[MessageKey.UnrecognizedParameters] = "Unrecognized parameters:{0}";
            messages[MessageKey.TooManyParameters] = "Too many parameters:{0}";
            messages[MessageKey.PleaseEnterTheFilePath] = "Please enter the file path:";
            messages[MessageKey.PleaseEnterTheFilePathUseDefaultInputPleaseEnterDirectly] = "Please enter the file path, Use default input ({0}), Please enter directly:";
            messages[MessageKey.TheInputFilePathDoesNotExist] = "The input file path {0} does not exist";
            messages[MessageKey.PleaseEnterTheMinimumNumberOfGroupsToDisplay] = "Please enter the minimum number of members to be displayed in the group, Use default input ({0})：";
            messages[MessageKey.PleaseEnterTheVerificationKeyValueOrTheFilePathWhereTheKeyValueIsStored] = "Please enter the verification key value or the file path where the key value is stored:";
            messages[MessageKey.PleaseEnterTheNumberOfScannedSubdirectoriesAtDifferentLevels] = "Please enter the number of scanned subdirectories at different levels, Use default input ({0}):";

            messages[MessageKey.Wait] = "Wait >>";
            messages[MessageKey.ThereIsNoSuchFile] = "There is no such file!";
            messages[MessageKey.ProcessCompleted] = "Process completed";
            messages[MessageKey.Matched] = "matched-{0}-{1}";
            messages[MessageKey.MatchedInKeysFile] = "matched-{0}-{1}-{2}.{3}";
            messages[MessageKey.Miss] = "miss-{0}";

            messages[MessageKey.NoKeyTheLengthIs] = "NoKeyTheLengthIs({0})";
            messages[MessageKey.NoKeyInFile] = "NoKeyInFile({0})";

            messages[MessageKey.ParameterLanguageUsageErrorMissingLanguageCode] = "Parameter -Language usage error, missing language code.";
            messages[MessageKey.ParameterErrorMissingPath] = "Parameter -{0} usage error, missing {0} path.";
            messages[MessageKey.ParameterError] = "Parameter -{0} usage error, Lack of necessary values {1}.";

            messages[MessageKey.MenuTitle] = "FileKey Menu";
            messages[MessageKey.MenuSetPath] = "Set file or directory path";
            messages[MessageKey.MenuSetKey] = "Set matching key value";
            messages[MessageKey.MenuSetOtherOptions] = "Set other options";
            messages[MessageKey.MenuConfigFiles] = "Configuration file";
            messages[MessageKey.MenuShowOptions] = "Show options";
            messages[MessageKey.MenuShowHelp] = "Help";
            messages[MessageKey.MenuRun] = "Run";
            messages[MessageKey.MenuClose] = "Close";
            messages[MessageKey.MenuReSet] = "Reset";
            messages[MessageKey.MenuSelected] = "Please select the menu and press Enter to confirm!";

            messages[MessageKey.SaveCurrentOptions] = "Save current options";
            messages[MessageKey.PleaseEnterTheConfigurationFileName] = "Please enter the configuration file name:";

        }

        private static void Initialize_zh()
        {
            messagesClear();
            messages[MessageKey.End] = "*结束*";
            messages[MessageKey.DisplayCompletedPressEnterToContinue] = "显示完毕，按回车继续下一个。";
            messages[MessageKey.UnrecognizedParameters] = "无法识别的参数：{0}";
            messages[MessageKey.TooManyParameters] = "参数过多：{0}";
            messages[MessageKey.PleaseEnterTheFilePath] = "请输入文件路径：";
            messages[MessageKey.PleaseEnterTheFilePathUseDefaultInputPleaseEnterDirectly] = "请输入文件路径，使用默认输入（{0}），请直接回车：";
            messages[MessageKey.TheInputFilePathDoesNotExist] = "输入的文件路径{0}不存在";
            messages[MessageKey.PleaseEnterTheMinimumNumberOfGroupsToDisplay] = "请输入需要显示的分组最少成员数，默认值（{0}）：";
            messages[MessageKey.PleaseEnterTheVerificationKeyValueOrTheFilePathWhereTheKeyValueIsStored] = "请输入验证Key值或存储Key值的文件路径：";
            messages[MessageKey.PleaseEnterTheNumberOfScannedSubdirectoriesAtDifferentLevels] = "请输入扫描子目录层级数量，默认值（{0}）：";

            messages[MessageKey.Wait] = "稍等 >>";
            messages[MessageKey.ThereIsNoSuchFile] = "没有这个文件！";
            messages[MessageKey.ProcessCompleted] = "处理完毕";
            messages[MessageKey.Matched] = "已匹配-{0}-{1}";
            messages[MessageKey.MatchedInKeysFile] = "已匹配-{0}-{1}-{2}.{3}";
            messages[MessageKey.Miss] = "不相同-{0}";

            messages[MessageKey.NoKeyTheLengthIs] = "没有Hash的长度是（{0}）";
            messages[MessageKey.NoKeyInFile] = "没有Hash在文件中被匹配（{0}）";

            messages[MessageKey.ParameterLanguageUsageErrorMissingLanguageCode] = "参数-Language使用错误，缺少语言代码。";
            messages[MessageKey.ParameterErrorMissingPath] = "参数-{0}使用错误，没有找到{0}的目录。";
            messages[MessageKey.ParameterError] = "参数-{0}使用错误， 缺少必要的附加参数{1}.";

            messages[MessageKey.MenuTitle] = "FileKey菜单";
            messages[MessageKey.MenuSetPath] = "设置文件或文件夹路径";
            messages[MessageKey.MenuSetKey] = "设置匹配Key值";
            messages[MessageKey.MenuSetOtherOptions] = "设置其他选项";
            messages[MessageKey.MenuConfigFiles] = "读写配置文件";
            messages[MessageKey.MenuShowOptions] = "显示已设置的选项";
            messages[MessageKey.MenuShowHelp] = "帮助";
            messages[MessageKey.MenuRun] = "开始";
            messages[MessageKey.MenuClose] = "关闭";
            messages[MessageKey.MenuReSet] = "重置";
            messages[MessageKey.MenuSelected] = "请选择菜单，并按回车键确认！";

            messages[MessageKey.SaveCurrentOptions] = "存储当前配置";
            messages[MessageKey.PleaseEnterTheConfigurationFileName] = "请输入配置文件名：";

            messages[MessageKey.Set] = "应用";
            messages[MessageKey.Del] = "删除";
        }

        public static string GetMessage(MessageKey message, params Object[] formatArgs)
        {
            var msg = messages[message];

            if (string.IsNullOrEmpty(msg))
                msg = Regex.Replace(message.ToString(), @"(?<!^)([A-Z])", " $1".ToLower());

            if (formatArgs.Length > 0)
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
 -t only {Type}
 -c only {Crc}
 -m only {Md5}
 -s only {Sha256}
";
        }

        public static List<string> GetMessageAll()
        {
            var allMsg = new List<string>();
            foreach (var msg in messages)
            {
                allMsg.Add($"{msg.Key}={msg.Value}{Environment.NewLine}");
            }
            return allMsg;
        }

        private static void LoadLanguage(string language)
        {
            var configType = AppOption.ConfigType.Language.ToString();
            var languageFile = Path.Combine(AppOption.GetConfigRootPath(configType), $"{configType}_{language}.txt");

            if (File.Exists(languageFile))
            {
                messagesClear();
                using (StreamReader reader = new StreamReader(languageFile))
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
                throw new Exception($"Could not find the corresponding {configType}_{language}.txt file for the language. Please create your own using the on-screen text style.");
            }
        }
    }
}
