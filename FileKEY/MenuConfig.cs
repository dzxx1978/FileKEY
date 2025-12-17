using static FileKEY.ConfigFile;
using static FileKEY.Language;

namespace FileKEY;

public class MenuConfig
{
    private List<string> options = new();

    /// <summary>
    /// 显示配置项主菜单
    /// </summary>
    /// <param name="args"></param>
    public void ShowMenu(string[]? args = null)
    {
        if (args is not null && args.Length > 0)
        {
            AppOption.SetOptions(args);
        }

        if (AppOption.IsPathFromArgs || AppOption.IsHelpShownAndExit)
            return;

        options = AppOption.GetOptions();

        var menuOptions = new string[] { };
        var menuSelected = 0;

        do
        {
            menuOptions = [
                GetMessage(MessageKey.MenuTitle),
                GetMessage(MessageKey.MenuSetPath),//1
                GetMessage(MessageKey.MenuSetKey),//2
                GetMessage(MessageKey.MenuSetOtherOptions),//3
                GetMessage(MessageKey.MenuConfigFiles),//4
                GetMessage(MessageKey.MenuShowOptions),//5
                GetMessage(MessageKey.MenuShowHelp),//6
                GetMessage(MessageKey.MenuReSet),//7
                GetMessage(MessageKey.MenuRun),
            ];
            menuSelected = Message.ShowSelectMenu(menuSelected > 0 ? menuSelected : menuOptions.Count() - 1, menuOptions);

            switch (menuSelected)
            {
                case 1:
                    showMenuPathOptions();
                    break;
                case 2:
                    showMenuKeyOptions();
                    break;
                case 3:
                    showMenuOtherOptions();
                    break;
                case 4:
                    showMenuConfigFilesOptions();
                    break;
                case 5:
                    Message.WriteLine(GetConfigString(options));
                    Message.Wait();
                    break;
                case 6:
                    Message.WriteLine(GetHelpShown());
                    Message.Wait();
                    break;
                case 7:
                    options.Clear();
                    Initialize();
                    break;
            }
        } while (menuSelected < menuOptions.Count() - 1);

        AppOption.SetOptions(options.ToArray());

    }

    private void showMenuLanguageOptions()
    {
        var languageSelected = 0;
        var menuOptions = new string[] { };
        var configType = ConfigType.Language;

        do
        {
            var languageFiles = GetConfigFiles(configType);

            menuOptions = [
                GetMessage(MessageKey.Language),
            GetMessage(MessageKey.SaveCurrentOptions),//1
            "en-US",//2
            "zh-CN",//3
            .. GetConfigFileNames(languageFiles),//4
            GetMessage(MessageKey.MenuClose),
        ];
            languageSelected = Message.ShowSelectMenu(languageSelected, menuOptions);

            switch (languageSelected)
            {
                case 1:

                    var configName = Message.ReadString(GetMessage(MessageKey.PleaseEnterTheConfigurationFileName));
                    var configFilePath = GetConfigFilePath(configType, configName, ["en-US", "en", "zh-CN", "zh"]);

                    var language = EditMessage();
                    Message.WriteLine(configFilePath);
                    var consoleKey = Message.Wait(GetMessage(MessageKey.SaveToFile) + "(y/n):", ConsoleKey.Y, ConsoleKey.N);
                    if (consoleKey.Key != ConsoleKey.Y)
                        break;

                    SaveConfigFile(configFilePath, language.ToArray());
                    break;
                case 2:
                case 3:
                    Initialize(menuOptions[languageSelected]);
                    break;
                default:
                    if (languageSelected < menuOptions.Count() - 1)
                    {
                        var selectedLanguageFilePath = languageFiles[languageSelected - 4];

                        var setOrDel = Message.ShowSelectMenu(0, [
                                selectedLanguageFilePath,
                            GetMessage(MessageKey.Set),//1
                            GetMessage(MessageKey.Del),//2
                            GetMessage(MessageKey.MenuClose),
                        ]);

                        switch (setOrDel)
                        {
                            case 1:
                                Initialize(GetConfigName(configType, selectedLanguageFilePath));
                                break;
                            case 2:
                                DeleteConfigFile(selectedLanguageFilePath);
                                break;
                        }

                    }
                    break;
            }
        } while (languageSelected < menuOptions.Count() - 1);

    }

    private void showMenuConfigFilesOptions()
    {

        var configSelected = 0;
        var menuOptions = new string[] { };
        var configType = ConfigType.Option;

        do
        {
            var configFiles = GetConfigFiles(configType);

            menuOptions = [
                GetMessage(MessageKey.MenuConfigFiles),
            GetMessage(MessageKey.SaveCurrentOptions),//1
            .. GetConfigFileNames(configFiles),//2
            GetMessage(MessageKey.MenuClose),
        ];

            configSelected = Message.ShowSelectMenu(configSelected, menuOptions);

            if (configSelected == 1)
            {

                var configName = Message.ReadString(GetMessage(MessageKey.PleaseEnterTheConfigurationFileName));
                var configFilePath = GetConfigFilePath(configType, configName);

                Message.WriteLine(GetConfigString(options));
                Message.WriteLine(configFilePath);

                var consoleKey = Message.Wait(GetMessage(MessageKey.SaveToFile) + "(y/n):", ConsoleKey.Y, ConsoleKey.N);
                if (consoleKey.Key != ConsoleKey.Y)
                    break;

                SaveConfigFile(configFilePath, options.ToArray());
            }
            else if (configSelected < menuOptions.Count() - 1)
            {
                var configFilePath = configFiles[configSelected - 2];

                var setOrDel = Message.ShowSelectMenu(0,
                    [
                        configFilePath,
                    GetMessage(MessageKey.Set),//1
                    GetMessage(MessageKey.Del),//2
                    GetMessage(MessageKey.MenuClose),
                ]);

                switch (setOrDel)
                {
                    case 1:
                        options = LoadConfigFile(configFilePath).ToList();

                        Message.WriteLine(GetConfigString(options));
                        Message.Wait();
                        break;
                    case 2:
                        DeleteConfigFile(configFilePath);
                        break;
                }

            }

        } while (configSelected < menuOptions.Count() - 1);

    }

    private void showMenuOtherOptions()
    {
        var optionSelected = 0;
        var menuOptions = new string[] { };

        do
        {
            menuOptions = [
                GetMessage(MessageKey.MenuSetOtherOptions),
            AppOption.Command_Equals.Substring(2),//"Equals",//1
            AppOption.Command_GroupBy.Substring(2),//"GroupBy",//2
            AppOption.Command_GroupMinCount.Substring(2),//"GroupMinCount",//3
            AppOption.Command_SubDirectory.Substring(2),//"SubDirectory",//4
            GetMessage(MessageKey.Hash),//5
            GetMessage(MessageKey.Display),//6
            GetMessage(MessageKey.Language),//7
            GetMessage(MessageKey.MenuClose),
        ];
            optionSelected = Message.ShowSelectMenu(optionSelected, menuOptions);

            switch (optionSelected)
            {
                case 1:
                    showMenuKeyOptions(AppOption.Command_Equals);
                    break;
                case 2:
                    showMenuGroupByOptions();
                    break;
                case 3:
                    showMenuGroupMinCountOptions();
                    break;
                case 4:
                    showMenuSubDirectoryOptions();
                    break;
                case 5:
                    showMenuHashOptions();
                    break;
                case 6:
                    showMenuDisplayOptions();
                    break;
                case 7:
                    showMenuLanguageOptions();
                    break;
            }

        } while (optionSelected < menuOptions.Count() - 1);

    }

    private void showMenuGroupByOptions()
    {
        var groupBySelected = 0;
        var menuOptions = new string[] { };

        do
        {
            var groupBy = 0;
            var fromMenuIndex = options.IndexOf(AppOption.Command_GroupBy);
            if (fromMenuIndex >= 0)
            {
                groupBy = options[fromMenuIndex + 1].ToLower() == "type" ? 2 : 3;
            }

            menuOptions = [
                AppOption.Command_GroupBy,
                "None",//1
                $"[{(groupBy==2 ? "T" : " ")}] Type",//2
                $"[{(groupBy==3 ? "H" : " ")}] Hash",//3
                GetMessage(MessageKey.MenuClose),
            ];

            groupBySelected = Message.ShowSelectMenu(groupBy, menuOptions);

            switch (groupBySelected)
            {
                case 1:
                    if (fromMenuIndex >= 0)
                    {
                        options.RemoveAt(fromMenuIndex);
                        options.RemoveAt(fromMenuIndex);
                    }
                    break;
                case 2:
                    options.Add(AppOption.Command_GroupBy);
                    options.Add("type");
                    goto case 1;
                case 3:
                    options.Add(AppOption.Command_GroupBy);
                    options.Add("hash");
                    goto case 1;
            }

        } while (groupBySelected < menuOptions.Count() - 1);

    }

    private void showMenuDisplayOptions()
    {
        var displaySelected = 0;
        var menuOptions = new string[] { };
        do
        {
            menuOptions = [
                $"-0 {GetMessage(MessageKey.Display)}",
            GetMessage(MessageKey.Detailed),//1
            $"[{(options.Contains("-0") ? "0" : " ")}] {GetMessage(MessageKey.Small)}",//2
            GetMessage(MessageKey.MenuClose),
        ];

            displaySelected = Message.ShowSelectMenu(displaySelected, menuOptions);

            switch (displaySelected)
            {
                case 1:
                    options.Remove("-0");
                    break;
                case 2:
                    options.Remove("-0");
                    options.Add("-0");
                    break;
            }

        } while (displaySelected < menuOptions.Count() - 1);
    }

    private void showMenuHashOptions()
    {
        var hashSelected = 0;
        var menuOptions = new string[] { };
        do
        {
            menuOptions = [
                $"-tcms {GetMessage(MessageKey.Hash)}",
            GetMessage(MessageKey.All),
            $"[{(options.Contains("-t") || options.Contains("-c") || options.Contains("-m") || options.Contains("-s") ? options.Contains("-t") ? "T" : " " : "t")}] {Language.Type}",
            $"[{(options.Contains("-t") || options.Contains("-c") || options.Contains("-m") || options.Contains("-s") ? options.Contains("-c") ? "C" : " " : "c")}] {Language.Crc}",
            $"[{(options.Contains("-t") || options.Contains("-c") || options.Contains("-m") || options.Contains("-s") ? options.Contains("-m") ? "M" : " " : "m")}] {Language.Md5}",
            $"[{(options.Contains("-t") || options.Contains("-c") || options.Contains("-m") || options.Contains("-s") ? options.Contains("-s") ? "S" : " " : "s")}] {Language.Sha256}",
            GetMessage(MessageKey.MenuClose),
        ];

            hashSelected = Message.ShowSelectMenu(hashSelected, menuOptions);

            switch (hashSelected)
            {
                case 1:
                    options.Remove("-t");
                    options.Remove("-c");
                    options.Remove("-m");
                    options.Remove("-s");
                    break;
                case 2:
                    if (options.Contains("-t"))
                    {
                        options.Remove("-t");
                    }
                    else
                    {
                        options.Add("-t");
                    }
                    break;
                case 3:
                    if (options.Contains("-c"))
                    {
                        options.Remove("-c");
                    }
                    else
                    {
                        options.Add("-c");
                    }
                    break;
                case 4:
                    if (options.Contains("-m"))
                    {
                        options.Remove("-m");
                    }
                    else
                    {
                        options.Add("-m");
                    }
                    break;
                case 5:
                    if (options.Contains("-s"))
                    {
                        options.Remove("-s");
                    }
                    else
                    {
                        options.Add("-s");
                    }
                    break;
            }
        } while (hashSelected < menuOptions.Count() - 1);

    }

    private void showMenuGroupMinCountOptions()
    {
        var groupMinCount = "1";
        var fromMenuIndex = options.IndexOf(AppOption.Command_GroupMinCount);
        if (fromMenuIndex >= 0)
        {
            groupMinCount = options[fromMenuIndex + 1];
            options.RemoveAt(fromMenuIndex);
            options.RemoveAt(fromMenuIndex);
        }

        int.TryParse(groupMinCount, out int outNum);

        groupMinCount = Message.ReadNumber(GetMessage(MessageKey.PleaseEnterTheMinimumNumberOfGroupsToDisplay, outNum), defaultValue: outNum);

        var isNum = int.TryParse(groupMinCount, out outNum);

        if (isNum && outNum > 1)
        {
            options.Add(AppOption.Command_GroupMinCount);
            options.Add(outNum.ToString());
        }
    }

    private void showMenuSubDirectoryOptions()
    {
        var subDirectory = "0";
        var fromMenuIndex = options.IndexOf(AppOption.Command_SubDirectory);
        if (fromMenuIndex >= 0)
        {
            subDirectory = options[fromMenuIndex + 1];
            options.RemoveAt(fromMenuIndex);
            options.RemoveAt(fromMenuIndex);
        }

        int.TryParse(subDirectory, out int outNum);

        subDirectory = Message.ReadNumber(GetMessage(MessageKey.PleaseEnterTheNumberOfScannedSubdirectoriesAtDifferentLevels, outNum), defaultValue: outNum);

        var isNum = int.TryParse(subDirectory, out outNum);

        if (isNum && outNum > 0)
        {
            options.Add(AppOption.Command_SubDirectory);
            options.Add(outNum.ToString());
        }
    }

    private void showMenuPathOptions()
    {
        var fileOrDirectoryPath = Message.ReadPath(GetMessage(MessageKey.PleaseEnterTheFilePath), AppOption.FileOrDirectoryPath);
        if (File.Exists(fileOrDirectoryPath))
        {
            options.Add(AppOption.Command_File);
        }
        else if (Directory.Exists(fileOrDirectoryPath))
        {
            options.Add(AppOption.Command_Directory);
        }
        else
        {
            Message.WarningLine(GetMessage(MessageKey.TheInputFilePathDoesNotExist, fileOrDirectoryPath));
            return;
        }

        var fromMenuIndex = options.IndexOf(AppOption.Command_PathFromMenu);
        if (fromMenuIndex > 0)
        {
            options.RemoveAt(fromMenuIndex);
            options.RemoveAt(fromMenuIndex - 1);
            options.RemoveAt(fromMenuIndex - 2);
        }
        options.Add(fileOrDirectoryPath);
        options.Add(AppOption.Command_PathFromMenu);

        AppOption.FileOrDirectoryPath = fileOrDirectoryPath;
    }

    private void showMenuKeyOptions(string optionName = "")
    {
        var fromMenuIndex = options.IndexOf(AppOption.Command_FileKeys);
        if (fromMenuIndex < 0)
        {
            fromMenuIndex = options.IndexOf(AppOption.Command_Key);
            if (fromMenuIndex < 0)
            {
                fromMenuIndex = options.IndexOf(AppOption.Command_Equals);
            }
        }
        if (fromMenuIndex >= 0)
        {
            options.RemoveAt(fromMenuIndex);
            options.RemoveAt(fromMenuIndex);
        }

        var comparisonKey = Message.ReadString(GetMessage(MessageKey.PleaseEnterTheVerificationKeyValueOrTheFilePathWhereTheKeyValueIsStored));
        if (optionName == AppOption.Command_Equals && File.Exists(comparisonKey))
        {
            options.Add(AppOption.Command_Equals);
        }
        else if (File.Exists(comparisonKey))
        {
            options.Add(AppOption.Command_FileKeys);
        }
        else if (!string.IsNullOrEmpty(comparisonKey))
        {
            options.Add(AppOption.Command_Key);
        }
        else
        {
            return;
        }
        options.Add(comparisonKey);
    }

    private List<string> EditMessage()
    {
        var languages = GetMessages();

        for (var i = 0; i < languages.Count; i++)
        {
            Message.Write($"{i + 1}/{languages.Count} {languages[i]}");
            var readLanguage = Message.ReadString("=");

            if (!string.IsNullOrEmpty(readLanguage))
            {
                languages[i] = languages[i].Split('=')[0] + "=" + readLanguage;
            }
        }

        return languages;
    }
}
