using System;
using System.Runtime.InteropServices;
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

        var menuOptions = Array.Empty<string>();
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

    private bool showMenuEditTypeOptions(string configFilePath)
    {
        var types = LoadConfigFile(configFilePath).ToList();

        var menuSelected = types.Count + 1;
        var menuOptions = Array.Empty<string>();

        do
        {
            menuOptions = [
                GetMessage(MessageEnum.Edit) + ">" + GetMessage(MessageEnum.Type),
                .. types,//1...
                GetMessage(MessageEnum.Add),//-3
                GetMessage(MessageEnum.SaveToFile),//-2
                GetMessage(MessageEnum.MenuClose),
            ];
            menuSelected = Message.ShowSelectMenu(menuSelected, menuOptions);

            if (menuSelected <= types.Count)
            {
                var editSelected = Message.ShowSelectMenu(0, [
                    types[menuSelected - 1],
                    GetMessage(MessageEnum.Edit),//1
                    GetMessage(MessageEnum.Del),//2
                    GetMessage(MessageEnum.MenuClose),
                ]);

                switch (editSelected)
                {
                    case 1:
                        var item = EditTypeItem(types[menuSelected - 1]);
                        if (!string.IsNullOrEmpty(item)) types[menuSelected - 1] = item;
                        break;
                    case 2:
                        types.RemoveAt(menuSelected - 1);
                        break;
                }
            }
            else if (menuSelected == menuOptions.Count() - 3)
            {
                var item = EditTypeItem();
                if (!string.IsNullOrEmpty(item)) types.Add(item);
            }
            else if (menuSelected == menuOptions.Count() - 2)
            {
                Message.WriteLine(configFilePath);
                var consoleKey = Message.Wait(GetMessage(MessageEnum.SaveToFile) + "(y/n):", ConsoleKey.Y, ConsoleKey.N);
                if (consoleKey.Key != ConsoleKey.Y)
                    break;

                SaveConfigFile(configFilePath, types.ToArray());

                return true;
            }
        } while (menuSelected < menuOptions.Count() - 1);

        return false;
    }

    private string EditTypeItem(string item = "")
    {
        var typeID = "";
        var typeNames = new List<string>();

        if (!string.IsNullOrEmpty(item))
        {
            typeID = item.Split('=')[0];
            typeNames = item.Substring(typeID.Length + 1).Split(';').Select(p => p.Trim()).ToList();
            Message.WriteLine($"{GetMessage(MessageEnum.Edit)} {GetMessage(MessageEnum.Type)} ID:");
        }
        else
        {
            Message.WriteLine($"{GetMessage(MessageEnum.Add)} {GetMessage(MessageEnum.Type)} ID:");
            typeID = Message.ReadString("+0>");
        }
        if (string.IsNullOrEmpty(typeID)) return item;

        Message.WarningLine(typeID, false);
        for (var i = 0; i < typeNames.Count; i++)
        {
            var newType = Message.ReadString($"{typeNames[i]}={i + 1}>");
            if (!string.IsNullOrEmpty(newType))
            {
                typeNames[i] = newType;
            }
        }

        while (true)
        {
            var newType = Message.ReadString($"+{typeNames.Count + 1}>");
            if (!string.IsNullOrEmpty(newType))
            {
                typeNames.Add(newType);
            }
            else
            {
                break;
            }
        }

        if (typeNames.Count > 0)
            item = $"{typeID}={string.Join("; ", typeNames.Where(p => !string.IsNullOrEmpty(p)).Select(p => p.Trim()))}";

        return item;
    }

    /// <summary>
    /// 编辑语言配置
    /// </summary>
    /// <returns></returns>
    private bool showMenuEditLanguagesOptions()
    {
        var languages = GetLanguages();

        var menuSelected = languages.Count + 1;
        var menuOptions = Array.Empty<string>();
        var configType = ConfigTypeEnum.Language;

        var configName = Message.ReadString(GetMessage(MessageEnum.PleaseEnterTheConfigurationFileName));
        var configFilePath = GetNewConfigFilePath(configType, configName, ["en-US", "en", "zh-CN", "zh"]);

        do
        {
            menuOptions = [
                GetMessage(MessageEnum.Edit) + ">" + GetMessage(MessageEnum.Language),
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
        var menuSelected = 0;
        var menuOptions = Array.Empty<string>();
        var configType = ConfigTypeEnum.Language;

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
            menuSelected = Message.ShowSelectMenu(menuSelected, menuOptions);

            switch (menuSelected)
            {
                case 1:
                    showMenuEditLanguagesOptions();
                    break;
                case 2:
                case 3:
                    Initialize(menuOptions[menuSelected]);
                    break;
                default:
                    if (menuSelected < menuOptions.Count() - 1)
                    {
                        var configFilePath = languageFiles[menuSelected - 4];

                        var setOrDel = Message.ShowSelectMenu(0, [
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
                                displayConfigFile(configFilePath);
                                break;
                            case 2:
                                Initialize(GetConfigName(configType, configFilePath));
                                break;
                            case 3:
                                DeleteConfigFile(configFilePath);
                                break;
                            case 4:
                                CopyToDefaultConfigFile(configFilePath);
                                goto case 2;
                        }

                    }
                    break;
            }
        } while (menuSelected < menuOptions.Count() - 1);

    }

    /// <summary>
    /// 显示状态设置配置菜单
    /// </summary>
    private void showMenuStatusFileOptions() {

        var menuSelected = 0;
        var menuOptions = Array.Empty<string>();
        var configType = ConfigTypeEnum.Status;

        do
        {
            var configFiles = GetConfigFiles(configType);

            menuOptions = [
                GetMessage(MessageEnum.MenuConfigFiles),
                GetMessage(MessageEnum.SaveCurrentOptions),//1
                .. GetConfigFileNames(configFiles),//2
                GetMessage(MessageEnum.MenuClose),
            ];

            menuSelected = Message.ShowSelectMenu(menuSelected, menuOptions);

            if (menuSelected == 1)
            {
                var configName = Message.ReadString(GetMessage(MessageEnum.PleaseEnterTheConfigurationFileName));
                var configFilePath = GetNewConfigFilePath(configType, configName);

                Message.WriteLine(GetConfigString(options));
                Message.WriteLine(configFilePath);

                var consoleKey = Message.Wait(GetMessage(MessageEnum.SaveToFile) + "(y/n):", ConsoleKey.Y, ConsoleKey.N);
                if (consoleKey.Key != ConsoleKey.Y)
                    break;

                SaveConfigFile(configFilePath, options.ToArray());
            }
            else if (menuSelected < menuOptions.Count() - 1)
            {
                var configFilePath = configFiles[menuSelected - 2];

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
                        displayConfigFile(configFilePath);
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

        } while (menuSelected < menuOptions.Count() - 1);

    }

    /// <summary>
    /// 显示默认设置配置菜单
    /// </summary>
    private void showMenuDefaultFileOptions()
    {

        var menuSelected = 0;
        var menuOptions = Array.Empty<string>();
        var configType = ConfigTypeEnum.Default;

        do
        {
            var configFiles = GetConfigFiles(configType);

            menuOptions = [
                GetMessage(MessageEnum.MenuConfigFiles),
                .. GetConfigFileNames(configFiles),//1
                GetMessage(MessageEnum.MenuClose),
            ];

            menuSelected = Message.ShowSelectMenu(menuSelected, menuOptions);

            if (menuSelected < menuOptions.Count() - 1)
            {
                var configFilePath = configFiles[menuSelected - 1];

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

        } while (menuSelected < menuOptions.Count() - 1);

    }

    /// <summary>
    /// 显示配置文件存盘读取菜单
    /// </summary>
    /// <returns></returns>
    private void showMenuConfigFilesOptions()
    {
        var menuSelected = 0;
        var menuOptions = Array.Empty<string>();

        do
        {
            menuOptions = [
                GetMessage(MessageEnum.MenuSetOtherOptions),
                GetMessage(MessageEnum.Status),//1
                GetMessage(MessageEnum.Language),//2
                GetMessage(MessageEnum.Type),//3
                GetMessage(MessageEnum.Default),//4
                GetMessage(MessageEnum.MenuClose),
            ];
            menuSelected = Message.ShowSelectMenu(menuSelected, menuOptions);

            switch (menuSelected)
            {
                case 1:
                    showMenuStatusFileOptions();
                    break;
                case 2:
                    showMenuLanguageFileOptions();
                    break;
                case 3:
                    showMenuTypeFileOptions();
                    break;
                case 4:
                    showMenuDefaultFileOptions();
                    break;
            }

        } while (menuSelected < menuOptions.Count() - 1);

    }

    private void showMenuTypeFileOptions()
    {
        var menuSelected = 0;
        var menuOptions = Array.Empty<string>();
        var configType = ConfigTypeEnum.Type;

        do
        {
            var configFiles = GetConfigFiles(configType);

            menuOptions = [
                GetMessage(MessageEnum.Edit) + ">" + GetMessage(MessageEnum.Type),
                GetMessage(MessageEnum.SaveCurrentOptions),//1
                .. GetConfigFileNames(configFiles),//2
                GetMessage(MessageEnum.MenuClose),
            ];
            menuSelected = Message.ShowSelectMenu(menuSelected, menuOptions);

            if (menuSelected == 1)
            {
                var configName = Message.ReadString(GetMessage(MessageEnum.PleaseEnterTheConfigurationFileName));
                var configFilePath = GetNewConfigFilePath(configType, configName);

                Message.WriteLine(configFilePath);

                var consoleKey = Message.Wait(GetMessage(MessageEnum.SaveToFile) + "(y/n):", ConsoleKey.Y, ConsoleKey.N);
                if (consoleKey.Key != ConsoleKey.Y)
                    break;

                SaveConfigFile(configFilePath, FileKey.GetFileTypes());
            }
            else if (menuSelected < menuOptions.Count() - 1)
            {
                var configFilePath = configFiles[menuSelected - 2];

                var setOrDel = Message.ShowSelectMenu(0,
                    [
                        configFilePath,
                        GetMessage(MessageEnum.Show),//1
                        GetMessage(MessageEnum.Set),//2
                        GetMessage(MessageEnum.Edit),//3
                        GetMessage(MessageEnum.Del),//4
                        GetMessage(MessageEnum.Default),//5
                        GetMessage(MessageEnum.MenuClose),
                    ]);

                switch (setOrDel)
                {
                    case 1:
                        displayConfigFile(configFilePath);
                        break;
                    case 2:
                        var types = LoadConfigFile(configFilePath);
                        FileKey.InitializeFileTypes(types);
                        break;
                    case 3:
                        showMenuEditTypeOptions(configFilePath);
                        break;
                    case 4:
                        DeleteConfigFile(configFilePath);
                        break;
                    case 5:
                        CopyToDefaultConfigFile(configFilePath);
                        break;
                }

            }

        } while (menuSelected < menuOptions.Count() - 1);
    }

    /// <summary>
    /// 读取并显示配置文件
    /// </summary>
    /// <param name="configFilePath">配置文件路径和文件名</param>
    private void displayConfigFile(string configFilePath) {
        var config = LoadConfigFile(configFilePath).ToList();
        Message.WriteLine(GetConfigString(config));
        Message.Wait(GetMessage(MessageEnum.DisplayCompletedPressEnterToContinue), ConsoleKey.Enter);
    }

    /// <summary>
    /// 显示其他设置菜单
    /// </summary>
    private void showMenuOtherOptions()
    {
        var menuSelected = 0;
        var menuOptions = Array.Empty<string>();

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
            menuSelected = Message.ShowSelectMenu(menuSelected, menuOptions);

            switch (menuSelected)
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

        } while (menuSelected < menuOptions.Count() - 1);

    }

    /// <summary>
    /// 显示分组设置菜单
    /// </summary>
    private void showMenuGroupByOptions()
    {
        var menuSelected = 0;
        var menuOptions = Array.Empty<string>();

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

            menuSelected = Message.ShowSelectMenu(groupBy.Equals(AppStatus.CommandValue_Type, StringComparison.OrdinalIgnoreCase) ? 2 : groupBy.Equals(AppStatus.CommandValue_Hash, StringComparison.OrdinalIgnoreCase) ? 3 : 0, menuOptions);

            switch (menuSelected)
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

        } while (menuSelected < menuOptions.Count() - 1);

    }

    /// <summary>
    /// 显示是否显示详细信息设置菜单
    /// </summary>
    private void showMenuDisplayOptions()
    {
        var menuSelected = 0;
        var menuOptions = Array.Empty<string>();
        do
        {
            menuOptions = [
                $"{AppStatus.Command_0} {GetMessage(MessageEnum.Display)}",
                GetMessage(MessageEnum.Detailed),//1
                $"[{(options.Contains(AppStatus.Command_0) ? AppStatus.Command_0.Substring(1) : " ")}] {GetMessage(MessageEnum.Small)}",//2
                GetMessage(MessageEnum.MenuClose),
            ];

            menuSelected = Message.ShowSelectMenu(menuSelected, menuOptions);

            switch (menuSelected)
            {
                case 1:
                    options.Remove(AppStatus.Command_0);
                    break;
                case 2:
                    optionsRemoveOrAdd(AppStatus.Command_0);
                    break;
            }

        } while (menuSelected < menuOptions.Count() - 1);
    }

    /// <summary>
    /// 显示输出Hash类型设置菜单
    /// </summary>
    private void showMenuHashOptions()
    {
        var menuSelected = 0;
        var menuOptions = Array.Empty<string>();
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

            menuSelected = Message.ShowSelectMenu(menuSelected, menuOptions);

            switch (menuSelected)
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
        } while (menuSelected < menuOptions.Count() - 1);

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

        var configFiles = GetConfigFiles(ConfigTypeEnum.Default);

        foreach (var configFile in configFiles)
        {
            var configFileName = Path.GetFileNameWithoutExtension(configFile);
            if (configFileName.EndsWith(ConfigTypeEnum.Status.ToString()))
            {
                options = LoadConfigFile(configFile).ToList();
                AppStatus.SetOptions(options.ToArray());
            }
            else if (configFileName.EndsWith(ConfigTypeEnum.Language.ToString()))
            {
                Initialize(GetConfigName(ConfigTypeEnum.Default, configFile));
            }
            else if (configFileName.EndsWith(ConfigTypeEnum.Type.ToString()))
            {
                var types = LoadConfigFile(configFile);
                FileKey.InitializeFileTypes(types);
            }
        }
    }
}
