using static FileKEY.Language;

namespace FileKEY;

public static class AppOption
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
    public static string FileOrDirectoryPath { get; set; } = "";

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
    public static bool IsDetailedInfoShown { get; private set; }

    /// <summary>
    /// 是否显示帮助并退出
    /// </summary>
    public static bool IsHelpShownAndExit { get; private set; }

    /// <summary>
    /// 是否是从命令行参数中获取的路径
    /// </summary>
    public static bool IsPathFromArgs { get; private set; }

    static AppOption()
    {
        initialize();
    }

    private static void initialize()
    {
        _outTypeOption = 1;
        _outCrcOption = 1;
        _outMd5Option = 1;
        _outSha256Option = 1;

        FileOrDirectoryPath = "";
        ComparisonKey = "";

        GroupBy = "";
        SubDirectory = 0;
        GroupMinCount = 1;

        IsDetailedInfoShown = true;
        IsHelpShownAndExit = false;
        IsPathFromArgs = false;
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
        __PathFromMenu,
        __Key,
        __FileKeys,
        __Equals,
        __GroupBy,
        __GroupMinCount,
        __GroupMaxCount,
        __SubDirectory,
        __Language,
        type,
        hash,
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
    public static string Command_PathFromMenu => Option.__PathFromMenu.ToCommand();
    public static string Command_Key => Option.__Key.ToCommand();
    public static string Command_FileKeys => Option.__FileKeys.ToCommand();
    public static string Command_Equals => Option.__Equals.ToCommand();
    public static string Command_GroupBy => Option.__GroupBy.ToCommand();
    public static string Command_GroupMinCount => Option.__GroupMinCount.ToCommand();
    public static string Command_SubDirectory => Option.__SubDirectory.ToCommand();
    public static string Command_Language => Option.__Language.ToCommand();


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

        if (!IsDetailedInfoShown)
        {
            options.Add(Command_0);
        }
        if (IsHelpShownAndExit)
        {
            options.Add(Command_v);
        }

        if (!string.IsNullOrEmpty(FileOrDirectoryPath))
        {
            if (File.Exists(FileOrDirectoryPath))
            {
                options.Add(Command_File);
                options.Add(FileOrDirectoryPath);
            }
            else
            {
                options.Add(Command_Directory);
                options.Add(FileOrDirectoryPath);
            }
            if (!IsPathFromArgs)
            {
                options.Add(Command_PathFromMenu);
            }
        }

        if (!string.IsNullOrEmpty(ComparisonKey))
        {
            if (File.Exists(ComparisonKey))
            {
                options.Add(Command_FileKeys);
                options.Add(ComparisonKey);
            }
            else if (Directory.Exists(ComparisonKey))
            {
                options.Add(Command_FileKeys);
                options.Add(ComparisonKey);
            }
            else
            {
                options.Add(Command_Key);
                options.Add(ComparisonKey);
            }
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
                            throw new Exception(GetMessage(MessageKey.ParameterLanguageUsageErrorMissingLanguageCode));
                        }

                    }
                    else if (parameter == Command_File)
                    {
                        i++;
                        if (i >= options.Length || !File.Exists(options[i]) || IsPathFromArgs)
                        {
                            throw new Exception(GetMessage(MessageKey.ParameterErrorMissingPath, parameter.Substring(1)));
                        }

                        IsPathFromArgs = true;
                        FileOrDirectoryPath = options[i];

                    }
                    else if (parameter == Command_Directory)
                    {

                        i++;
                        if (i >= options.Length || !Directory.Exists(options[i]) || IsPathFromArgs)
                        {
                            throw new Exception(GetMessage(MessageKey.ParameterErrorMissingPath, parameter.Substring(1)));
                        }

                        IsPathFromArgs = true;
                        FileOrDirectoryPath = options[i];

                    }
                    else if (parameter == Command_SubDirectory)
                    {

                        i++;
                        if (i >= options.Length || !int.TryParse(options[i], out int subDirectory))
                        {
                            throw new Exception(GetMessage(MessageKey.ParameterError, parameter.Substring(1), "<1|2|3|...>"));
                        }

                        SubDirectory = subDirectory;

                    }
                    else if (parameter == Command_FileKeys)
                    {
                        i++;
                        if (i >= options.Length || !File.Exists(options[i]))
                        {
                            throw new Exception(GetMessage(MessageKey.ParameterErrorMissingPath, parameter.Substring(1)));
                        }

                        if (string.IsNullOrEmpty(GroupBy))
                        {
                            ComparisonKey = options[i];

                        }

                    }
                    else if (parameter == Command_Key)
                    {

                        i++;
                        if (i >= options.Length || Directory.Exists(options[i]))
                        {
                            throw new Exception(GetMessage(MessageKey.ParameterErrorMissingPath, parameter.Substring(1)));
                        }

                        if (string.IsNullOrEmpty(GroupBy))
                        {
                            ComparisonKey = options[i];
                        }

                    }
                    else if (parameter == Command_Equals)
                    {
                        i++;
                        if (i >= options.Length || !File.Exists(options[i]))
                        {
                            throw new Exception(GetMessage(MessageKey.ParameterErrorMissingPath, parameter.Substring(1)));
                        }
                        if (!string.IsNullOrEmpty(GroupBy)) continue;

                        var fileKey = new FileKey(false, false, false, true);
                        ComparisonKey = fileKey.GetFileKeyInfo(options[i]).Result.Sha256Normalized;

                    }
                    else if (parameter == Command_GroupBy)
                    {
                        i++;
                        if (i >= options.Length || options[i].ToLower() != Option.type.ToString() && options[i].ToLower() != Option.hash.ToString())
                        {
                            throw new Exception(GetMessage(MessageKey.ParameterError, parameter.Substring(1), $"<{Option.type}|{Option.hash}>"));
                        }

                        if (options[i].ToLower() == Option.type.ToString())
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
                        IsDetailedInfoShown = false;
                        ComparisonKey = "";
                        GroupBy = options[i].ToLower();

                    }
                    else if (parameter == Command_GroupMinCount)
                    {

                        i++;
                        if (i >= options.Length || !int.TryParse(options[i], out int groupMinCount))
                        {
                            throw new Exception(GetMessage(MessageKey.ParameterError, parameter.Substring(1), "<1|2|3|...>"));
                        }

                        GroupMinCount = groupMinCount;

                    }
                    else if (parameter == Command_PathFromMenu)
                    {

                        IsPathFromArgs = false;

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
                            IsDetailedInfoShown = false;
                        }

                        if (!string.IsNullOrEmpty(parameter))
                        {
                            throw new Exception(GetMessage(MessageKey.UnrecognizedParameters, parameter));
                        }
                    }
                }
                else if (string.IsNullOrEmpty(FileOrDirectoryPath))
                {
                    IsPathFromArgs = true;
                    FileOrDirectoryPath = options[i];
                }
                else if (string.IsNullOrEmpty(ComparisonKey))
                {
                    if (string.IsNullOrEmpty(GroupBy))
                        ComparisonKey = options[i];
                }
                else
                {
                    throw new Exception(GetMessage(MessageKey.TooManyParameters, options[i]));
                }
            }

        }

        return;
    }

}
