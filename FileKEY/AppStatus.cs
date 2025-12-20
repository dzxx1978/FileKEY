using static FileKEY.Language;

namespace FileKEY;

public static class AppStatus
{
    private static int _outTypeOption;
    private static int _outCrcOption;
    private static int _outMd5Option;
    private static int _outSha256Option;

    private static int _outMaxOption => _outTypeOption == 2 || _outCrcOption == 2 || _outMd5Option == 2 || _outSha256Option == 2 ? 1 : 0;

    /// <summary>
    /// 是否仅有指定的哈希输出选项被启用
    /// </summary>
    /// <param name="hashName">哈希选项名称</param>
    /// <returns></returns>
    public static bool OnlyOutHashOption(string hashName)
    {

        var optionCount = 0;
        var optionName = "";

        if (OutCrcOption) { optionCount++; optionName = nameof(OutCrcOption); }
        if (OutMd5Option) { optionCount++; optionName = nameof(OutMd5Option); }
        if (OutSha256Option) { optionCount++; optionName = nameof(OutSha256Option); }

        if (optionCount == 1 && optionName == hashName)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 是否计算并输出文件类型
    /// </summary>
    public static bool OutTypeOption
    {
        get => _outTypeOption > _outMaxOption;
        private set => _outTypeOption = value ? 2 : 0;
    }

    /// <summary>
    /// 是否计算并输出CRC32哈希值
    /// </summary>
    public static bool OutCrcOption
    {
        get => _outCrcOption > _outMaxOption;
        private set => _outCrcOption = value ? 2 : 0;
    }

    /// <summary>
    /// 是否计算并输出MD5哈希值
    /// </summary>
    public static bool OutMd5Option
    {
        get => _outMd5Option > _outMaxOption;
        private set => _outMd5Option = value ? 2 : 0;
    }

    /// <summary>
    /// 是否计算并输出SHA256哈希值
    /// </summary>
    public static bool OutSha256Option
    {
        get => _outSha256Option > _outMaxOption;
        private set => _outSha256Option = value ? 2 : 0;
    }

    /// <summary>
    /// 是否为分组显示选项
    /// </summary>
    public static bool IsGroup => !string.IsNullOrEmpty(GroupBy);

    /// <summary>
    /// 文件或目录的路径
    /// </summary>
    public static string FileOrDirectoryPath { get; private set; } = "";

    /// <summary>
    /// 待比较的关键字或包含关键字的文本文件路径
    /// </summary>
    public static string ComparisonKey { get; private set; } = "";

    /// <summary>
    /// 分组显示方式（为空时正常显示）
    /// </summary>
    public static string GroupBy { get; private set; } = "";

    /// <summary>
    /// 扫描子目录的层级（0表示不扫描子目录）
    /// </summary>
    public static int SubDirectory { get; private set; }

    /// <summary>
    /// 分组显示时的最小数量
    /// </summary>
    public static int GroupMinCount { get; private set; }

    /// <summary>
    /// 是否显示详细信息
    /// </summary>
    public static bool IsDetailedDisplay { get; private set; }

    /// <summary>
    /// 是否显示帮助并退出
    /// </summary>
    public static bool IsHelpShownAndExit { get; private set; }

    /// <summary>
    /// 是否隐藏交互式界面
    /// </summary>
    public static bool IsHideMenu { get; private set; }

    /// <summary>
    /// 是否使用缓存数据
    /// </summary>
    public static bool IsCache { get; private set; }

    public enum FileTypeEnum
    {
        File,
        Directory,
        Noth,
    }

    private static FileTypeEnum fileType { get; set; } = FileTypeEnum.Noth;

    public enum KeyTypeEnum
    {
        Key,
        FileKeys,
        Equals,
        Noth,
    }

    private static KeyTypeEnum keyType { get; set; } = KeyTypeEnum.Noth;

    /// <summary>
    /// 文件类型
    /// </summary>
    public static bool IsFile => fileType == FileTypeEnum.File;
    /// <summary>
    /// 文件类型
    /// </summary>
    public static bool IsDirectory => fileType == FileTypeEnum.Directory;

    /// <summary>
    /// 是否是字符串关键字
    /// </summary>
    public static bool IsStringKey => keyType == KeyTypeEnum.Key;

    /// <summary>
    /// 是否是文本文件关键字
    /// </summary>
    public static bool IsTxtFileKeys => keyType == KeyTypeEnum.FileKeys;

    /// <summary>
    /// 是否是比对文件
    /// </summary>
    public static bool IsEqualsFile => keyType == KeyTypeEnum.Equals;


    static AppStatus()
    {
        initialize();
    }

    private static void initialize()
    {
        _outTypeOption = 1;
        _outCrcOption = 1;
        _outMd5Option = 1;
        _outSha256Option = 1;

        fileType = FileTypeEnum.Noth;
        FileOrDirectoryPath = "";

        keyType = KeyTypeEnum.Noth;
        ComparisonKey = "";

        GroupBy = "";
        SubDirectory = 0;
        GroupMinCount = 1;

        IsDetailedDisplay = true;
        IsHelpShownAndExit = false;
        IsHideMenu = false;
        IsCache = false;
    }

    private enum Option
    {
        _t,
        _c,
        _m,
        _s,
        _v,
        _0,
        __File,
        __Directory,
        __ShowMenu,
        __Key,
        __FileKeys,
        __Equals,
        __GroupBy,
        __GroupMinCount,
        __GroupMaxCount,
        __SubDirectory,
        __Language,
        Type,
        Hash,
        Noth,
        __Cache,
        True,
        False,
    }

    private static string ToCommand(this Option option)
    {
        return option.ToString().Replace("_", "-");
    }

    public static string Command_t => Option._t.ToCommand();
    public static string Command_c => Option._c.ToCommand();
    public static string Command_m => Option._m.ToCommand();
    public static string Command_s => Option._s.ToCommand();
    public static string Command_v => Option._v.ToCommand();
    public static string Command_0 => Option._0.ToCommand();
    public static string Command_File => Option.__File.ToCommand();
    public static string Command_Directory => Option.__Directory.ToCommand();
    public static string Command_ShowMenu => Option.__ShowMenu.ToCommand();
    public static string Command_Key => Option.__Key.ToCommand();
    public static string Command_FileKeys => Option.__FileKeys.ToCommand();
    public static string Command_Equals => Option.__Equals.ToCommand();
    public static string Command_GroupBy => Option.__GroupBy.ToCommand();
    public static string Command_GroupMinCount => Option.__GroupMinCount.ToCommand();
    public static string Command_SubDirectory => Option.__SubDirectory.ToCommand();
    public static string Command_Language => Option.__Language.ToCommand();
    public static string Command_Cache => Option.__Cache.ToCommand();

    public static string CommandValue_Type => Option.Type.ToString();
    public static string CommandValue_Hash => Option.Hash.ToString();
    public static string CommandValue_Noth => Option.Noth.ToString();

    public static string CommandValue_True => Option.True.ToString();
    public static string CommandValue_False => Option.False.ToString();

    /// <summary>
    /// 转换配置到命令行参数列表
    /// </summary>
    /// <returns></returns>
    public static List<string> GetOptions()
    {
        var options = new List<string>();
        if (_outTypeOption == 2)
        {
            options.Add(Command_t);
        }
        if (_outCrcOption == 2)
        {
            options.Add(Command_c);
        }
        if (_outMd5Option == 2)
        {
            options.Add(Command_m);
        }
        if (_outSha256Option == 2)
        {
            options.Add(Command_s);
        }

        if (!IsDetailedDisplay)
        {
            options.Add(Command_0);
        }
        if (IsHelpShownAndExit)
        {
            options.Add(Command_v);
        }

        if (!string.IsNullOrEmpty(FileOrDirectoryPath))
        {
            if (IsFile)
            {
                options.Add(Command_File);
            }
            else if (IsDirectory)
            {
                options.Add(Command_Directory);
            }
            options.Add(FileOrDirectoryPath);
            if (!IsHideMenu)
            {
                options.Add(Command_ShowMenu);
            }
        }

        if (!string.IsNullOrEmpty(ComparisonKey))
        {
            if (IsTxtFileKeys)
            {
                options.Add(Command_FileKeys);
            }
            else if (IsEqualsFile)
            {
                options.Add(Command_FileKeys);
            }
            else if (IsStringKey)
            {
                options.Add(Command_Key);
            }
            options.Add(ComparisonKey);
        }

        if (IsGroup)
        {
            options.Add(Command_GroupBy);
            options.Add(GroupBy);
        }

        if (GroupMinCount > 1)
        {
            options.Add(Command_GroupMinCount);
            options.Add(GroupMinCount.ToString());
        }

        if (SubDirectory > 0)
        {
            options.Add(Command_SubDirectory);
            options.Add(SubDirectory.ToString());
        }

        if (IsCache)
        {
            options.Add(Command_Cache);
            options.Add(IsCache.ToString());
        }

        return options;
    }

    /// <summary>
    /// 解析命令行参数到配置项
    /// </summary>
    /// <param name="options">参数列表</param>
    /// <exception cref="Exception"></exception>
    public static void SetOptions(string[] options)
    {
        initialize();

        if (options.Length > 0)
        {
            for (var i = 0; i < options.Length; i++)
            {
                if (string.IsNullOrEmpty(options[i])) continue;

                if (options[i].Substring(0, 1) == "-")
                {
                    var parameter = options[i];

                    if (parameter == Command_Language)
                    {
                        i++;
                        if (i < options.Length)
                        {
                            Initialize(options[i]);
                        }
                        else
                        {
                            throw new Exception(GetMessage(MessageEnum.ParameterLanguageUsageErrorMissingLanguageCode));
                        }

                    }
                    else if (parameter == Command_File)
                    {
                        i++;
                        if (i >= options.Length || fileType != FileTypeEnum.Noth || !SetFileOrDirectoryPath(options[i], FileTypeEnum.File))
                        {
                            throw new Exception(GetMessage(MessageEnum.ParameterErrorMissingPath, parameter.Substring(1)));
                        }

                    }
                    else if (parameter == Command_Directory)
                    {
                        i++;
                        if (i >= options.Length || fileType != FileTypeEnum.Noth || !SetFileOrDirectoryPath(options[i], FileTypeEnum.Directory))
                        {
                            throw new Exception(GetMessage(MessageEnum.ParameterErrorMissingPath, parameter.Substring(1)));
                        }

                    }
                    else if (parameter == Command_SubDirectory)
                    {

                        i++;
                        if (i >= options.Length || !int.TryParse(options[i], out int subDirectory))
                        {
                            throw new Exception(GetMessage(MessageEnum.ParameterError, parameter.Substring(1), "<1|2|3|...>"));
                        }

                        SubDirectory = subDirectory;

                    }
                    else if (parameter == Command_FileKeys)
                    {
                        i++;
                        if (i >= options.Length || !SetComparisonKey(options[i], KeyTypeEnum.FileKeys))
                        {
                            throw new Exception(GetMessage(MessageEnum.ParameterErrorMissingPath, parameter.Substring(1)));
                        }

                    }
                    else if (parameter == Command_Key)
                    {

                        i++;
                        if (i >= options.Length || !SetComparisonKey(options[i], KeyTypeEnum.Key))
                        {
                            throw new Exception(GetMessage(MessageEnum.ParameterErrorMissingPath, parameter.Substring(1)));
                        }

                    }
                    else if (parameter == Command_Equals)
                    {
                        i++;
                        if (i >= options.Length || !SetComparisonKey(options[i], KeyTypeEnum.Equals))
                        {
                            throw new Exception(GetMessage(MessageEnum.ParameterErrorMissingPath, parameter.Substring(1)));
                        }

                    }
                    else if (parameter == Command_GroupBy)
                    {
                        i++;
                        if (i >= options.Length || !options[i].Equals(CommandValue_Type, StringComparison.OrdinalIgnoreCase) && !options[i].Equals(CommandValue_Hash, StringComparison.OrdinalIgnoreCase))
                        {
                            throw new Exception(GetMessage(MessageEnum.ParameterError, parameter.Substring(1), $"<{CommandValue_Type}|{CommandValue_Hash}>"));
                        }

                        if (options[i].Equals(CommandValue_Type, StringComparison.OrdinalIgnoreCase))
                        {
                            _outTypeOption = 1;
                            _outCrcOption = 0;
                            _outMd5Option = 0;
                            _outSha256Option = 0;
                        }
                        else
                        {
                            _outTypeOption = 0;
                            _outCrcOption = 0;
                            _outMd5Option = 0;
                            _outSha256Option = 1;
                        }
                        IsDetailedDisplay = false;
                        SetComparisonKey("");
                        GroupBy = options[i].ToLower();

                    }
                    else if (parameter == Command_GroupMinCount)
                    {
                        i++;
                        if (i >= options.Length || !int.TryParse(options[i], out int groupMinCount))
                        {
                            throw new Exception(GetMessage(MessageEnum.ParameterError, parameter.Substring(1), "<1|2|3|...>"));
                        }

                        GroupMinCount = groupMinCount;

                    }
                    else if (parameter == Command_Cache)
                    {
                        i++;
                        if (i >= options.Length || !options[i].Equals(CommandValue_True, StringComparison.OrdinalIgnoreCase) && !options[i].Equals(CommandValue_True, StringComparison.OrdinalIgnoreCase))
                        {
                            throw new Exception(GetMessage(MessageEnum.ParameterError, parameter.Substring(1), $"<{CommandValue_True}|{CommandValue_False}>"));
                        }

                        IsCache = options[i].Equals(CommandValue_True, StringComparison.OrdinalIgnoreCase) ? true : false;

                    }
                    else if (parameter == Command_ShowMenu)
                    {
                        IsHideMenu = false;

                    }
                    else
                    {
                        parameter = parameter.Substring(1).ToLower();
                        if (parameter.Contains(Command_v.Substring(1)))
                        {
                            IsHelpShownAndExit = true;
                            return;
                        }
                        if (parameter.Contains(Command_t.Substring(1)))
                        {
                            parameter = parameter.Replace(Command_t.Substring(1), "");
                            OutTypeOption = true;
                        }
                        if (parameter.Contains(Command_c.Substring(1)))
                        {
                            parameter = parameter.Replace(Command_c.Substring(1), "");
                            OutCrcOption = true;
                        }
                        if (parameter.Contains(Command_m.Substring(1)))
                        {
                            parameter = parameter.Replace(Command_m.Substring(1), "");
                            OutMd5Option = true;
                        }
                        if (parameter.Contains(Command_s.Substring(1)))
                        {
                            parameter = parameter.Replace(Command_s.Substring(1), "");
                            OutSha256Option = true;
                        }
                        if (parameter.Contains(Command_0.Substring(1)))
                        {
                            parameter = parameter.Replace(Command_0.Substring(1), "");
                            IsDetailedDisplay = false;
                        }

                        if (!string.IsNullOrEmpty(parameter))
                        {
                            throw new Exception(GetMessage(MessageEnum.UnrecognizedParameters, parameter));
                        }
                    }
                }
                else if (fileType == FileTypeEnum.Noth)
                {
                    SetFileOrDirectoryPath(options[i]);
                }
                else if (keyType == KeyTypeEnum.Noth)
                {
                    SetComparisonKey(options[i]);
                }
                else
                {
                    throw new Exception(GetMessage(MessageEnum.TooManyParameters, options[i]));
                }
            }

        }

        return;
    }

    public static bool SetFileOrDirectoryPath(string path, FileTypeEnum setType = FileTypeEnum.Noth, bool setHideMenu = true)
    {
        if (string.IsNullOrEmpty(path))
        {
            fileType = FileTypeEnum.Noth;
            FileOrDirectoryPath = "";
            return true;
        }

        if ((setType == FileTypeEnum.Noth || setType == FileTypeEnum.File) && File.Exists(path))
        {
            fileType = FileTypeEnum.File;
        }
        else if ((setType == FileTypeEnum.Noth || setType == FileTypeEnum.Directory) && Directory.Exists(path))
        {
            fileType = FileTypeEnum.Directory;
        }
        else
        {
            fileType = FileTypeEnum.Noth;
            FileOrDirectoryPath = "";
            throw new Exception(GetMessage(MessageEnum.UnrecognizedParameters, path));
        }

        if (setHideMenu) IsHideMenu = true;
        FileOrDirectoryPath = path;
        return true;
    }

    public static bool SetComparisonKey(string comparisonKey, KeyTypeEnum setType = KeyTypeEnum.Noth)
    {
        keyType = KeyTypeEnum.Noth;
        ComparisonKey = "";
        if (IsGroup || string.IsNullOrEmpty(comparisonKey)) return false;

        if (setType == KeyTypeEnum.Key)
        {
            keyType = setType;
        }
        else
        {
            var isFile = File.Exists(comparisonKey);

            if (setType == KeyTypeEnum.Noth)
            {
                if (isFile)
                {
                    keyType = KeyTypeEnum.FileKeys;
                }
                else
                {
                    keyType = KeyTypeEnum.Key;
                }
            }
            else if (!isFile)
            {
                return false;
            }
            else
            {
                keyType = setType;
            }
        }

        ComparisonKey = comparisonKey;
        return true;
    }

}
