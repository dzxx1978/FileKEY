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
            AppStatus.SetOptions(args);
            if (AppStatus.IsHideMenu || AppStatus.IsHelpShownAndExit)
                return;
        }
        else
        {
            setDefaultConfig();
        }
        options = AppStatus.GetOptions();

        var menuOptions = new string[] { };
        var menuSelected = 0;

        do
        {
            menuOptions = [
                GetMessage(MessageEnum.MenuTitle),
                GetMessage(MessageEnum.MenuSetPath),//1
                GetMessage(MessageEnum.MenuSetKey),//2
                GetMessage(MessageEnum.MenuSetOtherOptions),//3
                GetMessage(MessageEnum.MenuConfigFiles),//4
                GetMessage(MessageEnum.MenuShowOptions),//5
                GetMessage(MessageEnum.MenuShowHelp),//6
                GetMessage(MessageEnum.MenuReSet),//7
                GetMessage(MessageEnum.MenuRun),
            ];
            menuSelected = Message.ShowSelectMenu(menuSelected > 0 ? menuSelected : menuOptions.Count() - 1, menuOptions);

            switch (menuSelected)
            {
                case 1:
                    readConsolePathOptions();
                    break;
                case 2:
                    readConsoleKeyOptions();
                    break;
                case 3:
                    showMenuOtherOptions();
                    break;
                case 4:
                    showMenuConfigFilesOptions();
                    break;
                case 5:
                    Message.WriteLine(GetConfigString(options));
                    Message.Wait(GetMessage(MessageEnum.DisplayCompletedPressEnterToContinue), ConsoleKey.Enter);
                    break;
                case 6:
                    Message.WriteLine(GetHelpShown());
                    Message.Wait(GetMessage(MessageEnum.DisplayCompletedPressEnterToContinue), ConsoleKey.Enter);
                    break;
                case 7:
                    options.Clear();
                    Initialize();
                    break;
            }
        } while (menuSelected < menuOptions.Count() - 1);

        AppStatus.SetOptions(options.ToArray());

    }

    /// <summary>
    /// 编辑语言配置
    /// </summary>
    /// <returns></returns>
    private bool showMenuEditLanguagesOptions()
    {
        var languages = GetLanguages();

        var menuSelected = languages.Count + 1;
        var menuOptions = new string[] { };
        var configType = ConfigType.Language;

        var configName = Message.ReadString(GetMessage(MessageEnum.PleaseEnterTheConfigurationFileName));
        var configFilePath = GetConfigFilePath(configType, configName, ["en-US", "en", "zh-CN", "zh"]);

        do
        {
            var languageFiles = GetConfigFiles(configType);

            menuOptions = [
                GetMessage(MessageEnum.Language),
                .. languages,//1...
                GetMessage(MessageEnum.SaveToFile),
                GetMessage(MessageEnum.MenuClose),
            ];
            menuSelected = Message.ShowSelectMenu(menuSelected, menuOptions);

            if (menuSelected <= languages.Count)
            {
                var readLanguage = Message.ReadString("=>");
                if (!string.IsNullOrEmpty(readLanguage))
                {
                    languages[menuSelected - 1] = $"{languages[menuSelected - 1].Split('=')[0]}={readLanguage}";
                }
            }
            else if (menuSelected == menuOptions.Count() - 1)
            {
                break;
            }
            else {
                Message.WriteLine(configFilePath);
                var consoleKey = Message.Wait(GetMessage(MessageEnum.SaveToFile) + "(y/n):", ConsoleKey.Y, ConsoleKey.N);
                if (consoleKey.Key != ConsoleKey.Y)
                    break;

                SaveConfigFile(configFilePath, languages.ToArray());

                return true;
            }
        } while (menuSelected < menuOptions.Count() - 1);

        return false;
    }
    /// <summary>
    /// 显示语言文字存盘读取编辑菜单
    /// </summary>
    private void showMenuLanguageFileOptions()
    {
        var languageSelected = 0;
        var menuOptions = new string[] { };
        var configType = ConfigType.Language;

        do
        {
            var languageFiles = GetConfigFiles(configType);

            menuOptions = [
                GetMessage(MessageEnum.Language),
                GetMessage(MessageEnum.SaveCurrentOptions),//1
                "en-US",//2
                "zh-CN",//3
                .. GetConfigFileNames(languageFiles),//4
                GetMessage(MessageEnum.MenuClose),
            ];
            languageSelected = Message.ShowSelectMenu(languageSelected, menuOptions);

            switch (languageSelected)
            {
                case 1:
                    showMenuEditLanguagesOptions();
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
                            GetMessage(MessageEnum.Show),//1
                            GetMessage(MessageEnum.Set),//2
                            GetMessage(MessageEnum.Del),//3
                            GetMessage(MessageEnum.Default),//4
                            GetMessage(MessageEnum.MenuClose),
                        ]);

                        switch (setOrDel)
                        {
                            case 1:
                                var language = LoadConfigFile(selectedLanguageFilePath);
                                Message.WriteLine(GetConfigString(language.ToList()));
                                Message.Wait(GetMessage(MessageEnum.DisplayCompletedPressEnterToContinue), ConsoleKey.Enter);
                                break;
                            case 2:
                                Initialize(GetConfigName(configType, selectedLanguageFilePath));
                                break;
                            case 3:
                                DeleteConfigFile(selectedLanguageFilePath);
                                break;
                            case 4:
                                CopyToDefaultConfigFile(selectedLanguageFilePath);
                                goto case 2;
                        }

                    }
                    break;
            }
        } while (languageSelected < menuOptions.Count() - 1);

    }

    /// <summary>
    /// 显示状态设置配置菜单
    /// </summary>
    private void showMenuStatusFileOptions() {

        var configSelected = 0;
        var menuOptions = new string[] { };
        var configType = ConfigType.Status;

        do
        {
            var configFiles = GetConfigFiles(configType);

            menuOptions = [
                GetMessage(MessageEnum.MenuConfigFiles),
                GetMessage(MessageEnum.SaveCurrentOptions),//1
                .. GetConfigFileNames(configFiles),//2
                GetMessage(MessageEnum.MenuClose),
            ];

            configSelected = Message.ShowSelectMenu(configSelected, menuOptions);

            if (configSelected == 1)
            {

                var configName = Message.ReadString(GetMessage(MessageEnum.PleaseEnterTheConfigurationFileName));
                var configFilePath = GetConfigFilePath(configType, configName);

                Message.WriteLine(GetConfigString(options));
                Message.WriteLine(configFilePath);

                var consoleKey = Message.Wait(GetMessage(MessageEnum.SaveToFile) + "(y/n):", ConsoleKey.Y, ConsoleKey.N);
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
                        GetMessage(MessageEnum.Show),//1
                        GetMessage(MessageEnum.Set),//2
                        GetMessage(MessageEnum.Del),//3
                        GetMessage(MessageEnum.Default),//4
                        GetMessage(MessageEnum.MenuClose),
                    ]);

                switch (setOrDel)
                {
                    case 1:
                        var language = LoadConfigFile(configFilePath).ToList();
                        Message.WriteLine(GetConfigString(language));
                        Message.Wait(GetMessage(MessageEnum.DisplayCompletedPressEnterToContinue), ConsoleKey.Enter);
                        break;
                    case 2:
                        options = LoadConfigFile(configFilePath).ToList();
                        Message.WriteLine(GetConfigString(options));
                        Message.Wait(GetMessage(MessageEnum.DisplayCompletedPressEnterToContinue), ConsoleKey.Enter);
                        break;
                    case 3:
                        DeleteConfigFile(configFilePath);
                        break;
                    case 4:
                        CopyToDefaultConfigFile(configFilePath);
                        break;
                }

            }

        } while (configSelected < menuOptions.Count() - 1);

    }

    /// <summary>
    /// 显示默认设置配置菜单
    /// </summary>
    private void showMenuDefaultFileOptions()
    {

        var configSelected = 0;
        var menuOptions = new string[] { };
        var configType = ConfigType.Default;

        do
        {
            var configFiles = GetConfigFiles(configType);

            menuOptions = [
                GetMessage(MessageEnum.MenuConfigFiles),
                .. GetConfigFileNames(configFiles),//1
                GetMessage(MessageEnum.MenuClose),
            ];

            configSelected = Message.ShowSelectMenu(configSelected, menuOptions);

            if (configSelected < menuOptions.Count() - 1)
            {
                var configFilePath = configFiles[configSelected - 1];

                var setOrDel = Message.ShowSelectMenu(0,
                    [
                        configFilePath,
                        GetMessage(MessageEnum.Del),//1
                        GetMessage(MessageEnum.MenuClose),
                    ]);

                switch (setOrDel)
                {
                    case 1:
                        DeleteConfigFile(configFilePath);
                        break;
                }

            }

        } while (configSelected < menuOptions.Count() - 1);

    }

    /// <summary>
    /// 显示配置文件存盘读取菜单
    /// </summary>
    /// <returns></returns>
    private void showMenuConfigFilesOptions()
    {
        var optionSelected = 0;
        var menuOptions = new string[] { };

        do
        {
            menuOptions = [
                GetMessage(MessageEnum.MenuSetOtherOptions),
                GetMessage(MessageEnum.Status),//1
                GetMessage(MessageEnum.Language),//2
                GetMessage(MessageEnum.Default),//3
                GetMessage(MessageEnum.MenuClose),
            ];
            optionSelected = Message.ShowSelectMenu(optionSelected, menuOptions);

            switch (optionSelected)
            {
                case 1:
                    showMenuStatusFileOptions();
                    break;
                case 2:
                    showMenuLanguageFileOptions();
                    break;
                case 3:
                    showMenuDefaultFileOptions();
                    break;
            }

        } while (optionSelected < menuOptions.Count() - 1);

    }

    /// <summary>
    /// 显示其他设置菜单
    /// </summary>
    private void showMenuOtherOptions()
    {
        var optionSelected = 0;
        var menuOptions = new string[] { };

        do
        {
            menuOptions = [
                GetMessage(MessageEnum.MenuSetOtherOptions),
                AppStatus.Command_Equals.Substring(2),//"Equals",//1
                AppStatus.Command_GroupBy.Substring(2),//"GroupBy",//2
                AppStatus.Command_GroupMinCount.Substring(2),//"GroupMinCount",//3
                AppStatus.Command_SubDirectory.Substring(2),//"SubDirectory",//4
                GetMessage(MessageEnum.Hash),//5
                GetMessage(MessageEnum.Display),//6
                GetMessage(MessageEnum.MenuClose),
            ];
            optionSelected = Message.ShowSelectMenu(optionSelected, menuOptions);

            switch (optionSelected)
            {
                case 1:
                    readConsoleKeyOptions(AppStatus.KeyTypeEnum.Equals);
                    break;
                case 2:
                    showMenuGroupByOptions();
                    break;
                case 3:
                    readConsoleGroupMinCountOptions();
                    break;
                case 4:
                    readConsoleSubDirectoryOptions();
                    break;
                case 5:
                    showMenuHashOptions();
                    break;
                case 6:
                    showMenuDisplayOptions();
                    break;
            }

        } while (optionSelected < menuOptions.Count() - 1);

    }

    /// <summary>
    /// 显示分组设置菜单
    /// </summary>
    private void showMenuGroupByOptions()
    {
        var groupBySelected = 0;
        var menuOptions = new string[] { };

        do
        {
            var groupBy = optionsGet(AppStatus.Command_GroupBy, 2);
            if (string.IsNullOrEmpty(groupBy)) groupBy = AppStatus.CommandValue_Noth;

            menuOptions = [
                AppStatus.Command_GroupBy,
                AppStatus.CommandValue_Noth,//1
                $"[{(groupBy.Equals(AppStatus.CommandValue_Type,StringComparison.OrdinalIgnoreCase) ? AppStatus.CommandValue_Type.Substring(0,1).ToUpper() : " ")}] {AppStatus.CommandValue_Type}",//2
                $"[{(groupBy.Equals(AppStatus.CommandValue_Hash,StringComparison.OrdinalIgnoreCase) ? AppStatus.CommandValue_Hash.Substring(0,1).ToUpper() : " ")}] {AppStatus.CommandValue_Hash}",//3
                GetMessage(MessageEnum.MenuClose),
            ];

            groupBySelected = Message.ShowSelectMenu(groupBy.Equals(AppStatus.CommandValue_Type, StringComparison.OrdinalIgnoreCase) ? 2 : groupBy.Equals(AppStatus.CommandValue_Hash, StringComparison.OrdinalIgnoreCase) ? 3 : 0, menuOptions);

            switch (groupBySelected)
            {
                case 1:
                    optionsRemove(AppStatus.Command_GroupBy, 2);
                    break;
                case 2:
                    optionsReplaceOrAdd(AppStatus.Command_GroupBy, AppStatus.CommandValue_Type);
                    break;
                case 3:
                    optionsReplaceOrAdd(AppStatus.Command_GroupBy, AppStatus.CommandValue_Hash);
                    break;
            }

        } while (groupBySelected < menuOptions.Count() - 1);

    }

    /// <summary>
    /// 显示是否显示详细信息设置菜单
    /// </summary>
    private void showMenuDisplayOptions()
    {
        var displaySelected = 0;
        var menuOptions = new string[] { };
        do
        {
            menuOptions = [
                $"{AppStatus.Command_0} {GetMessage(MessageEnum.Display)}",
                GetMessage(MessageEnum.Detailed),//1
                $"[{(options.Contains(AppStatus.Command_0) ? AppStatus.Command_0.Substring(1) : " ")}] {GetMessage(MessageEnum.Small)}",//2
                GetMessage(MessageEnum.MenuClose),
            ];

            displaySelected = Message.ShowSelectMenu(displaySelected, menuOptions);

            switch (displaySelected)
            {
                case 1:
                    options.Remove(AppStatus.Command_0);
                    break;
                case 2:
                    optionsRemoveOrAdd(AppStatus.Command_0);
                    break;
            }

        } while (displaySelected < menuOptions.Count() - 1);
    }

    /// <summary>
    /// 显示输出Hash类型设置菜单
    /// </summary>
    private void showMenuHashOptions()
    {
        var hashSelected = 0;
        var menuOptions = new string[] { };
        do
        {
            menuOptions = [
                $"{AppStatus.Command_t}{AppStatus.Command_c.Substring(1)}{AppStatus.Command_m.Substring(1)}{AppStatus.Command_s.Substring(1)} {GetMessage(MessageEnum.Hash)}",
                GetMessage(MessageEnum.All),
                $"[{(options.Contains(AppStatus.Command_t) || options.Contains(AppStatus.Command_c) || options.Contains(AppStatus.Command_m) || options.Contains(AppStatus.Command_s) ? options.Contains(AppStatus.Command_t) ? AppStatus.Command_t.Substring(1).ToUpper() : " " : AppStatus.Command_t.Substring(1).ToLower())}] {Language.Type}",
                $"[{(options.Contains(AppStatus.Command_t) || options.Contains(AppStatus.Command_c) || options.Contains(AppStatus.Command_m) || options.Contains(AppStatus.Command_s) ? options.Contains(AppStatus.Command_c) ? AppStatus.Command_c.Substring(1).ToUpper() : " " : AppStatus.Command_c.Substring(1).ToLower())}] {Language.Crc}",
                $"[{(options.Contains(AppStatus.Command_t) || options.Contains(AppStatus.Command_c) || options.Contains(AppStatus.Command_m) || options.Contains(AppStatus.Command_s) ? options.Contains(AppStatus.Command_m) ? AppStatus.Command_m.Substring(1).ToUpper() : " " : AppStatus.Command_m.Substring(1).ToLower())}] {Language.Md5}",
                $"[{(options.Contains(AppStatus.Command_t) || options.Contains(AppStatus.Command_c) || options.Contains(AppStatus.Command_m) || options.Contains(AppStatus.Command_s) ? options.Contains(AppStatus.Command_s) ? AppStatus.Command_s.Substring(1).ToUpper() : " " : AppStatus.Command_s.Substring(1).ToLower())}] {Language.Sha256}",
                GetMessage(MessageEnum.MenuClose),
            ];

            hashSelected = Message.ShowSelectMenu(hashSelected, menuOptions);

            switch (hashSelected)
            {
                case 1:
                    options.Remove(AppStatus.Command_t);
                    options.Remove(AppStatus.Command_c);
                    options.Remove(AppStatus.Command_m);
                    options.Remove(AppStatus.Command_s);
                    break;
                case 2:
                    optionsRemoveOrAdd(AppStatus.Command_t);
                    break;
                case 3:
                    optionsRemoveOrAdd(AppStatus.Command_c);
                    break;
                case 4:
                    optionsRemoveOrAdd(AppStatus.Command_m);
                    break;
                case 5:
                    optionsRemoveOrAdd(AppStatus.Command_s);
                    break;
            }
        } while (hashSelected < menuOptions.Count() - 1);

    }

    /// <summary>
    /// 从控制台获取显示分组最小项目数设置输入
    /// </summary>
    private void readConsoleGroupMinCountOptions()
    {
        var groupMinCount = optionsRemove(AppStatus.Command_GroupMinCount, 2);
        int.TryParse(groupMinCount, out int outNum);

        groupMinCount = Message.ReadNumber(GetMessage(MessageEnum.PleaseEnterTheMinimumNumberOfGroupsToDisplay, outNum), defaultValue: outNum);
        var isNum = int.TryParse(groupMinCount, out outNum);

        if (isNum && outNum > 1)
        {
            options.Add(AppStatus.Command_GroupMinCount);
            options.Add(outNum.ToString());
        }
    }

    /// <summary>
    /// 从控制台获取扫描子目录层数设置输入
    /// </summary>
    private void readConsoleSubDirectoryOptions()
    {
        var subDirectory = optionsRemove(AppStatus.Command_SubDirectory, 2);
        int.TryParse(subDirectory, out int outNum);

        subDirectory = Message.ReadNumber(GetMessage(MessageEnum.PleaseEnterTheNumberOfScannedSubdirectoriesAtDifferentLevels, outNum), defaultValue: outNum);
        var isNum = int.TryParse(subDirectory, out outNum);

        if (isNum && outNum > 0)
        {
            options.Add(AppStatus.Command_SubDirectory);
            options.Add(outNum.ToString());
        }
    }

    /// <summary>
    /// 从控制台获取文件文件夹路径配置输入
    /// </summary>
    private void readConsolePathOptions()
    {
        var fileOrDirectoryPath = Message.ReadPath(GetMessage(MessageEnum.PleaseEnterTheFilePath), string.Empty);

        optionsRemove(AppStatus.Command_ShowMenu);
        optionsRemove(AppStatus.Command_File, 2);
        optionsRemove(AppStatus.Command_Directory, 2);

        if (string.IsNullOrEmpty(fileOrDirectoryPath)) return;

        if (File.Exists(fileOrDirectoryPath))
        {
            options.Add(AppStatus.Command_File);
        }
        else if (Directory.Exists(fileOrDirectoryPath))
        {
            options.Add(AppStatus.Command_Directory);
        }
        else
        {
            Message.WarningLine(GetMessage(MessageEnum.TheInputFilePathDoesNotExist, fileOrDirectoryPath));
            return;
        }
        options.Add(fileOrDirectoryPath);
        options.Add(AppStatus.Command_ShowMenu);

    }

    /// <summary>
    /// 从控制台获取验证值配置输入
    /// </summary>
    /// <param name="setType"></param>
    private void readConsoleKeyOptions(AppStatus.KeyTypeEnum setType = AppStatus.KeyTypeEnum.Noth)
    {
        if (options.Contains(AppStatus.Command_GroupBy)) return;

        optionsRemove(AppStatus.Command_FileKeys, 2);
        optionsRemove(AppStatus.Command_Key, 2);
        optionsRemove(AppStatus.Command_Equals, 2);

        var comparisonKey = Message.ReadString(GetMessage(MessageEnum.PleaseEnterTheVerificationKeyValueOrTheFilePathWhereTheKeyValueIsStored));
        if (string.IsNullOrEmpty(comparisonKey)) return;

        if (setType == AppStatus.KeyTypeEnum.Equals)
        {
            if (File.Exists(comparisonKey))
            {
                options.Add(AppStatus.Command_Equals);
            }
            else
            {
                Message.WarningLine(GetMessage(MessageEnum.TheInputFilePathDoesNotExist, $"\"{AppStatus.Command_Equals} {comparisonKey}\""));
                return;
            }
        }
        else if (File.Exists(comparisonKey))
        {
            options.Add(AppStatus.Command_FileKeys);
        }
        else
        {
            options.Add(AppStatus.Command_Key);
        }

        options.Add(comparisonKey);

    }

    /// <summary>
    /// 获得指定配置项的值
    /// </summary>
    /// <param name="item">配置名称</param>
    /// <param name="count">值所在偏移行</param>
    /// <returns></returns>
    private string optionsGet(string item, int count = 1)
    {
        var outOption = "";
        var fromMenuIndex = options.IndexOf(item);
        if (fromMenuIndex >= 0)
        {
            if (options.Count > fromMenuIndex + count - 1)
                outOption = options[fromMenuIndex + count - 1];
        }

        return outOption;
    }

    /// <summary>
    /// 移除指定名称配置项，并返回最后一条被移除的配置内容。
    /// </summary>
    /// <param name="item">移除名称</param>
    /// <param name="count">连续移除行数</param>
    /// <returns>最后一条被移除的行</returns>
    private string optionsRemove(string item, int count = 1)
    {
        var outOption = "";
        var fromMenuIndex = options.IndexOf(item);
        if (fromMenuIndex >= 0)
        {
            for (var i = 1; i <= count; i++)
            {
                if (options.Count == fromMenuIndex) break;
                if (i == count) outOption = options[fromMenuIndex];
                options.RemoveAt(fromMenuIndex);
            }
        }

        return outOption;
    }

    /// <summary>
    /// 移除或添加配置项（存在时移除，不存在时添加）
    /// </summary>
    /// <param name="item">项目名称</param>
    /// <returns></returns>
    private bool optionsRemoveOrAdd(string item)
    {
        if (options.Contains(item))
        {
            options.Remove(item);
            return false;
        }
        else
        {
            options.Add(item);
            return true;
        }
    }

    /// <summary>
    /// 移除旧项目并添加新项目
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    private bool optionsRemoveAndAdd(params string[] items)
    {
        if (items is null || items.Length == 0) return false;

        optionsRemove(items[0], items.Length);
        foreach (var item in items)
        {
            options.Add(item);
        }

        return true;
    }

    /// <summary>
    /// 替换或新增项目和值，并返回被替换的值
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    private string optionsReplaceOrAdd(params string[] items)
    {
        if (items is null || items.Length == 0 || string.IsNullOrEmpty(items[0])) return string.Empty;

        var outOption = "";
        var itemsIndex = options.IndexOf(items[0]);
        if (itemsIndex >= 0)
        {
            if (itemsIndex + items.Length > options.Count) return string.Empty;

            for (var i = 1; i < items.Length; i++)
            {
                if (i == items.Length) outOption = options[itemsIndex + i];
                options[itemsIndex + i] = items[i];
            }
        }
        else
        {
            foreach (var item in items)
            {
                options.Add(item);
            }
        }

        return outOption;
    }

    private void setDefaultConfig()
    {

        var configFiles = GetConfigFiles(ConfigType.Default);

        foreach (var configFile in configFiles)
        {
            if (Path.GetFileNameWithoutExtension(configFile).EndsWith(ConfigType.Status.ToString()))
            {
                options = LoadConfigFile(configFile).ToList();
                AppStatus.SetOptions(options.ToArray());
            }
            else if (Path.GetFileNameWithoutExtension(configFile).EndsWith(ConfigType.Language.ToString()))
            {
                Initialize(GetConfigName(ConfigType.Default, configFile));
            }

        }
    }
}
