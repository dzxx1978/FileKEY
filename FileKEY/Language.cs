using System.Globalization;
using System.Text.RegularExpressions;
using static FileKEY.ConfigFile;

namespace FileKEY;

public static class Language
{
    public static readonly string Type = "type";
    public static readonly string Crc = "CRC32";
    public static readonly string Md5 = "MD5";
    public static readonly string Sha256 = "SHA256";

    private static Dictionary<MessageEnum, string> messages = new();

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

    private static void messagesClear()
    {

        messages.Clear();
        foreach (MessageEnum msg in Enum.GetValues(typeof(MessageEnum)))
        {
            messages.Add(msg, "");
        }

    }

    public enum MessageEnum
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
        messages[MessageEnum.End] = "*END*";
        messages[MessageEnum.DisplayCompletedPressEnterToContinue] = "Display completed, press enter to continue.";
        messages[MessageEnum.UnrecognizedParameters] = "Unrecognized parameters:{0}";
        messages[MessageEnum.TooManyParameters] = "Too many parameters:{0}";
        messages[MessageEnum.PleaseEnterTheFilePath] = "Please enter the file path:";
        messages[MessageEnum.PleaseEnterTheFilePathUseDefaultInputPleaseEnterDirectly] = "Please enter the file path, Use default input ({0}), Please enter directly:";
        messages[MessageEnum.TheInputFilePathDoesNotExist] = "The input file path {0} does not exist";
        messages[MessageEnum.PleaseEnterTheMinimumNumberOfGroupsToDisplay] = "Please enter the minimum number of members to be displayed in the group, Use default input ({0})：";
        messages[MessageEnum.PleaseEnterTheVerificationKeyValueOrTheFilePathWhereTheKeyValueIsStored] = "Please enter the verification key value or the file path where the key value is stored:";
        messages[MessageEnum.PleaseEnterTheNumberOfScannedSubdirectoriesAtDifferentLevels] = "Please enter the number of scanned subdirectories at different levels, Use default input ({0}):";

        messages[MessageEnum.Wait] = "Wait >>";
        messages[MessageEnum.ThereIsNoSuchFile] = "There is no such file!";
        messages[MessageEnum.ProcessCompleted] = "Process completed";
        messages[MessageEnum.Matched] = "matched-{0}-{1}";
        messages[MessageEnum.MatchedInKeysFile] = "matched-{0}-{1}-{2}.{3}";
        messages[MessageEnum.Miss] = "miss-{0}";

        messages[MessageEnum.NoKeyTheLengthIs] = "NoKeyTheLengthIs({0})";
        messages[MessageEnum.NoKeyInFile] = "NoKeyInFile({0})";

        messages[MessageEnum.ParameterLanguageUsageErrorMissingLanguageCode] = "Parameter -Language usage error, missing language code.";
        messages[MessageEnum.ParameterErrorMissingPath] = "Parameter -{0} usage error, missing {0} path.";
        messages[MessageEnum.ParameterError] = "Parameter -{0} usage error, Lack of necessary values {1}.";

        messages[MessageEnum.MenuTitle] = "FileKey Menu";
        messages[MessageEnum.MenuSetPath] = "Set file or directory path";
        messages[MessageEnum.MenuSetKey] = "Set matching key value";
        messages[MessageEnum.MenuSetOtherOptions] = "Set other options";
        messages[MessageEnum.MenuConfigFiles] = "Configuration file";
        messages[MessageEnum.MenuShowOptions] = "Show options";
        messages[MessageEnum.MenuShowHelp] = "Help";
        messages[MessageEnum.MenuRun] = "Run";
        messages[MessageEnum.MenuClose] = "Close";
        messages[MessageEnum.MenuReSet] = "Reset";
        messages[MessageEnum.MenuSelected] = "Please select the menu and press Enter to confirm!";

        messages[MessageEnum.SaveCurrentOptions] = "Save current options";
        messages[MessageEnum.PleaseEnterTheConfigurationFileName] = "Please enter the configuration file name:";

    }

    private static void Initialize_zh()
    {
        messagesClear();
        messages[MessageEnum.End] = "*结束*";
        messages[MessageEnum.DisplayCompletedPressEnterToContinue] = "显示完毕，按回车继续下一个。";
        messages[MessageEnum.UnrecognizedParameters] = "无法识别的参数：{0}";
        messages[MessageEnum.TooManyParameters] = "参数过多：{0}";
        messages[MessageEnum.PleaseEnterTheFilePath] = "请输入文件路径：";
        messages[MessageEnum.PleaseEnterTheFilePathUseDefaultInputPleaseEnterDirectly] = "请输入文件路径，使用默认输入（{0}），请直接回车：";
        messages[MessageEnum.TheInputFilePathDoesNotExist] = "输入的文件路径{0}不存在";
        messages[MessageEnum.PleaseEnterTheMinimumNumberOfGroupsToDisplay] = "请输入需要显示的分组最少成员数，默认值（{0}）：";
        messages[MessageEnum.PleaseEnterTheVerificationKeyValueOrTheFilePathWhereTheKeyValueIsStored] = "请输入验证Key值或存储Key值的文件路径：";
        messages[MessageEnum.PleaseEnterTheNumberOfScannedSubdirectoriesAtDifferentLevels] = "请输入扫描子目录层级数量，默认值（{0}）：";

        messages[MessageEnum.Wait] = "稍等 >>";
        messages[MessageEnum.ThereIsNoSuchFile] = "没有这个文件！";
        messages[MessageEnum.ProcessCompleted] = "处理完毕";
        messages[MessageEnum.Matched] = "已匹配-{0}-{1}";
        messages[MessageEnum.MatchedInKeysFile] = "已匹配-{0}-{1}-{2}.{3}";
        messages[MessageEnum.Miss] = "不相同-{0}";

        messages[MessageEnum.NoKeyTheLengthIs] = "没有Hash的长度是（{0}）";
        messages[MessageEnum.NoKeyInFile] = "没有Hash在文件中被匹配（{0}）";

        messages[MessageEnum.ParameterLanguageUsageErrorMissingLanguageCode] = "参数-Language使用错误，缺少语言代码。";
        messages[MessageEnum.ParameterErrorMissingPath] = "参数-{0}使用错误，没有找到{0}的目录。";
        messages[MessageEnum.ParameterError] = "参数-{0}使用错误， 缺少必要的附加参数{1}.";

        messages[MessageEnum.MenuTitle] = "FileKey菜单";
        messages[MessageEnum.MenuSetPath] = "设置文件或文件夹路径";
        messages[MessageEnum.MenuSetKey] = "设置匹配Key值";
        messages[MessageEnum.MenuSetOtherOptions] = "设置其他选项";
        messages[MessageEnum.MenuConfigFiles] = "读写配置文件";
        messages[MessageEnum.MenuShowOptions] = "显示已设置的选项";
        messages[MessageEnum.MenuShowHelp] = "帮助";
        messages[MessageEnum.MenuRun] = "开始";
        messages[MessageEnum.MenuClose] = "关闭";
        messages[MessageEnum.MenuReSet] = "重置";
        messages[MessageEnum.MenuSelected] = "请选择菜单，并按回车键确认！";

        messages[MessageEnum.SaveCurrentOptions] = "存储当前配置";
        messages[MessageEnum.PleaseEnterTheConfigurationFileName] = "请输入配置文件名：";

        messages[MessageEnum.Set] = "应用";
        messages[MessageEnum.Del] = "删除";
    }

    public static string GetMessage(MessageEnum message, params Object[] formatArgs)
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

    public static List<string> GetLanguages()
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
        var configFilePath = GetConfigFilePath(ConfigType.Language, language);
        var lines = LoadConfigFileAsync(configFilePath).Result;

        if (lines.Length > 0)
        {
            messagesClear();

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;

                var item = line.Split('=');
                if (item.Length == 2)
                {
                    messages[Enum.Parse<MessageEnum>(item[0])] = item[1];
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
            throw new Exception($"Could not find the corresponding {Path.GetFileName(configFilePath)} file for the language. Please create your own using the on-screen text style.");
        }
    }
}
