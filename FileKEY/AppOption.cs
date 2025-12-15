using static FileKEY.Language;

namespace FileKEY
{
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

        /// <summary>
        /// 解析命令行参数
        /// </summary>
        /// <param name="options">参数列表</param>
        /// <exception cref="Exception"></exception>
        public static List<string> SetOptions(string[] options)
        {
            initialize();

            var outOptions = new List<string>();
            if (options.Length > 0)
            {
                for (var i = 0; i < options.Length; i++)
                {
                    if (options[i].Substring(0, 1) == "-")
                    {
                        var parameter = options[i].Substring(1);

                        if (parameter == "-Language")
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
                        else if (parameter == "-File")
                        {
                            i++;
                            if (i >= options.Length || !File.Exists(options[i]) || IsPathFromArgs)
                            {
                                throw new Exception(GetMessage(MessageKey.ParameterErrorMissingPath, parameter.Substring(1)));
                            }

                            IsPathFromArgs = true;
                            FileOrDirectoryPath = options[i];

                            outOptions.Add($"--File");
                            outOptions.Add(options[i]);

                        }
                        else if (parameter == "-Directory")
                        {

                            i++;
                            if (i >= options.Length || !Directory.Exists(options[i]) || IsPathFromArgs)
                            {
                                throw new Exception(GetMessage(MessageKey.ParameterErrorMissingPath, parameter.Substring(1)));
                            }

                            IsPathFromArgs = true;
                            FileOrDirectoryPath = options[i];

                            outOptions.Add($"--Directory");
                            outOptions.Add(options[i]);

                        }
                        else if (parameter == "-SubDirectory")
                        {

                            i++;
                            if (i >= options.Length || !int.TryParse(options[i], out int subDirectory))
                            {
                                throw new Exception(GetMessage(MessageKey.ParameterError, parameter.Substring(1), "<1|2|3|...>"));
                            }

                            SubDirectory = subDirectory;

                            outOptions.Add($"--SubDirectory");
                            outOptions.Add(subDirectory.ToString());

                        }
                        else if (parameter == "-FileKeys")
                        {
                            i++;
                            if (i >= options.Length || !File.Exists(options[i]))
                            {
                                throw new Exception(GetMessage(MessageKey.ParameterErrorMissingPath, parameter.Substring(1)));
                            }

                            if (string.IsNullOrEmpty(GroupBy))
                            {
                                ComparisonKey = options[i];

                                outOptions.Add($"--FileKeys");
                                outOptions.Add(options[i]);

                            }

                        }
                        else if (parameter == "-Key")
                        {

                            i++;
                            if (i >= options.Length || Directory.Exists(options[i]))
                            {
                                throw new Exception(GetMessage(MessageKey.ParameterErrorMissingPath, parameter.Substring(1)));
                            }

                            if (string.IsNullOrEmpty(GroupBy))
                            {
                                ComparisonKey = options[i];

                                outOptions.Add($"--Key");
                                outOptions.Add(options[i]);

                            }

                        }
                        else if (parameter == "-Equals")
                        {
                            i++;
                            if (i >= options.Length || !File.Exists(options[i]))
                            {
                                throw new Exception(GetMessage(MessageKey.ParameterErrorMissingPath, parameter.Substring(1)));
                            }
                            if (!string.IsNullOrEmpty(GroupBy)) continue;

                            var fileKey = new FileKey(false, false, false, true);
                            ComparisonKey = fileKey.GetFileKeyInfo(options[i]).Result.Sha256Normalized;

                            outOptions.Add($"--Key");
                            outOptions.Add(ComparisonKey);

                        }
                        else if (parameter == "-GroupBy")
                        {
                            i++;
                            if (i >= options.Length || options[i].ToLower() != "type" && options[i].ToLower() != "hash")
                            {
                                throw new Exception(GetMessage(MessageKey.ParameterError, parameter.Substring(1), "<type|hash>"));
                            }

                            if (options[i].ToLower() == "type")
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

                            outOptions.Add($"--GroupBy");
                            outOptions.Add(GroupBy);

                        }
                        else if (parameter == "-GroupMinCount")
                        {

                            i++;
                            if (i >= options.Length || !int.TryParse(options[i], out int groupMinCount))
                            {
                                throw new Exception(GetMessage(MessageKey.ParameterError, parameter.Substring(1), "<1|2|3|...>"));
                            }

                            GroupMinCount = groupMinCount;

                            outOptions.Add($"--GroupMinCount");
                            outOptions.Add(GroupMinCount.ToString());


                        }
                        else if (parameter == "-PathFromMenu")
                        {

                            IsPathFromArgs = false;

                        }
                        else
                        {
                            parameter = parameter.ToUpper();
                            if (parameter.Contains("V"))
                            {
                                IsHelpShownAndExit = true;
                                return outOptions;
                            }
                            if (parameter.Contains("T"))
                            {
                                parameter = parameter.Replace("T", "");
                                OutTypeOption = true;
                                outOptions.Add($"-t");
                            }
                            if (parameter.Contains("C"))
                            {
                                parameter = parameter.Replace("C", "");
                                OutCrcOption = true;
                                outOptions.Add($"-c");
                            }
                            if (parameter.Contains("M"))
                            {
                                parameter = parameter.Replace("M", "");
                                OutMd5Option = true;
                                outOptions.Add($"-m");
                            }
                            if (parameter.Contains("S"))
                            {
                                parameter = parameter.Replace("S", "");
                                OutSha256Option = true;
                                outOptions.Add($"-s");
                            }
                            if (parameter.Contains("0"))
                            {
                                parameter = parameter.Replace("0", "");
                                IsDetailedInfoShown = false;
                                outOptions.Add($"-0");
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

            return outOptions;
        }

        public static void ShowMenu(List<string> options)
        {
            options = SetOptions(options.ToArray());

            if (IsPathFromArgs || IsHelpShownAndExit)
                return;

            var menuOptions = new string[]
            {
                GetMessage(MessageKey.MenuTitle),
                GetMessage(MessageKey.MenuSetPath),//1
                GetMessage(MessageKey.MenuSetKey),//2
                GetMessage(MessageKey.MenuSetOptions),//3
                GetMessage(MessageKey.MenuShowOptions),//4
                GetMessage(MessageKey.MenuShowHelp),//5
                GetMessage(MessageKey.MenuReSet),//6
                GetMessage(MessageKey.MenuRun),
            };
            var menuSelected = menuOptions.Count() - 1;

            do
            {
                menuSelected = Message.ShowSelectMenu(menuSelected, menuOptions);

                switch (menuSelected)
                {
                    case 1:
                        setPath(options);
                        break;
                    case 2:
                        setKey(options);
                        break;
                    case 3:
                        showMenuOptions(options);
                        break;
                    case 4:
                        if (options.Count == 0)
                            Message.Write("None");

                        foreach (var option in options)
                        {
                            Message.WriteLine(option);
                        }
                        Message.Wait();
                        break;
                    case 5:
                        Message.WriteLine(Language.GetHelpShown());
                        Message.Wait();
                        break;
                    case 6:
                        options.Clear();
                        break;
                }
            } while (menuSelected < menuOptions.Count() - 1);

            SetOptions(options.ToArray());

        }

        private static void showMenuOptions(List<string> options)
        {
            var optionSelected = 0;
            var menuOptions = new string[]
            {
                GetMessage(MessageKey.MenuSetOptions),
                "Equals",//1
                "GroupBy",//2
                "GroupMinCount",//3
                "SubDirectory",//4
                "Hash",//5
                GetMessage(MessageKey.MenuClose),
            };

            do
            {
                optionSelected = Message.ShowSelectMenu(optionSelected, menuOptions);

                switch (optionSelected)
                {
                    case 1:
                        setKey(options, "Equals");
                        break;
                    case 2:
                        showMenuGroupByOptions(options);
                        break;
                    case 3:
                        showMenuGroupMinCountOptions(options);
                        break;
                    case 4:
                        showMenuSubDirectoryOptions(options);
                        break;
                    case 5:
                        showMenuHashOptions(options);
                        break;
                }
            } while (optionSelected < menuOptions.Count() - 1);
        }

        private static void showMenuGroupByOptions(List<string> options)
        {
            var groupBy = 0;
            var fromMenuIndex = options.IndexOf("--GroupBy");
            if (fromMenuIndex >= 0)
            {
                int.TryParse(options[fromMenuIndex + 1].ToLower() == "type" ? "2" : "3", out groupBy);
                options.RemoveAt(fromMenuIndex);
                options.RemoveAt(fromMenuIndex);
            }

            var groupBySelected = Message.ShowSelectMenu(groupBy,
            [
                "--GroupBy",
                "None",//1
                "Type",//2
                "Hash",//3
            ]);

            switch (groupBySelected)
            {
                case 1:
                    break;
                case 2:
                    options.Add("--GroupBy");
                    options.Add("type");
                    break;
                case 3:
                    options.Add("--GroupBy");
                    options.Add("hash");
                    break;
            }
        }


        private static void showMenuHashOptions(List<string> options)
        {
            var hashSelected = 0;
            var menuOptions = new string[] { };
            do
            {
                menuOptions = [
                    "--Hash",
                    "All",
                    $"({(options.Contains("-t") || options.Contains("-c") || options.Contains("-m") || options.Contains("-s") ? options.Contains("-t") ? "V" : "X" : "v")}) {Language.Type}",
                    $"({(options.Contains("-t") || options.Contains("-c") || options.Contains("-m") || options.Contains("-s") ? options.Contains("-c") ? "V" : "X" : "v")}) {Language.Crc}",
                    $"({(options.Contains("-t") || options.Contains("-c") || options.Contains("-m") || options.Contains("-s") ? options.Contains("-m") ? "V" : "X" : "v")}) {Language.Md5}",
                    $"({(options.Contains("-t") || options.Contains("-c") || options.Contains("-m") || options.Contains("-s") ? options.Contains("-s") ? "V" : "X" : "v")}) {Language.Sha256}",
                    GetMessage(MessageKey.MenuClose),
                ];

                hashSelected = Message.ShowSelectMenu(hashSelected, menuOptions);

                switch (hashSelected)
                {
                    case 1:
                        if (options.Contains("-t") || options.Contains("-c") || options.Contains("-m") || options.Contains("-s"))
                        {
                            options.Remove("-t");
                            options.Remove("-c");
                            options.Remove("-m");
                            options.Remove("-s");
                        }
                        else
                        {
                            options.Add("-t");
                            options.Add("-c");
                            options.Add("-m");
                            options.Add("-s");
                        }
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
        private static void showMenuGroupMinCountOptions(List<string> options)
        {
            var groupMinCount = "0";
            var fromMenuIndex = options.IndexOf("--GroupMinCount");
            if (fromMenuIndex >= 0)
            {
                groupMinCount = options[fromMenuIndex + 1];
                options.RemoveAt(fromMenuIndex);
                options.RemoveAt(fromMenuIndex);
            }

            int.TryParse(groupMinCount, out int outNum);

            groupMinCount = Message.ReadNumber(GetMessage(MessageKey.PleaseEnterTheMinimumNumberOfGroupsToDisplay, outNum), defaultValue: outNum);

            var isNum = int.TryParse(groupMinCount, out outNum);

            if (isNum && outNum > 0)
            {
                options.Add("--GroupMinCount");
                options.Add(outNum.ToString());
            }
        }

        private static void showMenuSubDirectoryOptions(List<string> options)
        {
            var subDirectory = "0";
            var fromMenuIndex = options.IndexOf("--SubDirectory");
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
                options.Add("--SubDirectory");
                options.Add(outNum.ToString());
            }
        }

        private static void setPath(List<string> options)
        {
            var fileOrDirectoryPath = Message.ReadPath(GetMessage(MessageKey.PleaseEnterTheFilePath), AppOption.FileOrDirectoryPath);
            if (File.Exists(fileOrDirectoryPath))
            {
                options.Add("--File");
            }
            else if (Directory.Exists(fileOrDirectoryPath))
            {
                options.Add("--Directory");
            }
            else
            {
                Message.WarningLine(GetMessage(MessageKey.TheInputFilePathDoesNotExist, fileOrDirectoryPath));
                return;
            }

            var fromMenuIndex = options.IndexOf("--PathFromMenu");
            if (fromMenuIndex > 0)
            {
                options.RemoveAt(fromMenuIndex);
                options.RemoveAt(fromMenuIndex - 1);
                options.RemoveAt(fromMenuIndex - 2);
            }
            options.Add(fileOrDirectoryPath);
            options.Add("--PathFromMenu");

            FileOrDirectoryPath = fileOrDirectoryPath;
        }

        private static void setKey(List<string> options, string optionName = "")
        {
            var fromMenuIndex = options.IndexOf("--FileKeys");
            if (fromMenuIndex < 0)
            {
                fromMenuIndex = options.IndexOf("--Key");
                if (fromMenuIndex < 0)
                {
                    fromMenuIndex = options.IndexOf("--Equals");
                }
            }
            if (fromMenuIndex >= 0)
            {
                options.RemoveAt(fromMenuIndex);
                options.RemoveAt(fromMenuIndex);
            }

            var comparisonKey = Message.ReadString(GetMessage(MessageKey.PleaseEnterTheVerificationKeyValueOrTheFilePathWhereTheKeyValueIsStored));
            if (optionName == "Equals" && File.Exists(comparisonKey))
            {
                options.Add("--Equals");
            }
            else if (File.Exists(comparisonKey))
            {
                options.Add("--FileKeys");
            }
            else if (!string.IsNullOrEmpty(comparisonKey))
            {
                options.Add("--Key");
            }
            else
            {
                return;
            }
            options.Add(comparisonKey);
        }
    }
}
