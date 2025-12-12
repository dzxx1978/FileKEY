using static FileKEY.Language;

namespace FileKEY
{
    public static class AppOption
    {
        private static int _outTypeOption;
        private static int _outCrcOption;
        private static int _outMd5Option;
        private static int _outSha256Option;

        private static int _outMaxOption { get => _outTypeOption == 2 || _outCrcOption == 2 || _outMd5Option == 2 || _outSha256Option == 2 ? 1 : 0; }

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
            set => _outTypeOption = value ? 2 : 0;
        }

        /// <summary>
        /// 是否计算并输出CRC32哈希值
        /// </summary>
        public static bool OutCrcOption
        {
            get => _outCrcOption > _outMaxOption;
            set => _outCrcOption = value ? 2 : 0;
        }

        /// <summary>
        /// 是否计算并输出MD5哈希值
        /// </summary>
        public static bool OutMd5Option
        {
            get => _outMd5Option > _outMaxOption;
            set => _outMd5Option = value ? 2 : 0;
        }

        /// <summary>
        /// 是否计算并输出SHA256哈希值
        /// </summary>
        public static bool OutSha256Option
        {
            get => _outSha256Option > _outMaxOption;
            set => _outSha256Option = value ? 2 : 0;
        }

        /// <summary>
        /// 文件或目录的路径
        /// </summary>
        public static string FileOrDirectoryPath { get; set; } = "";

        /// <summary>
        /// 待比较的关键字或包含关键字的文本文件路径
        /// </summary>
        public static string ComparisonKey { get; set; } = "";

        /// <summary>
        /// 分组显示方式（为空时正常显示）
        /// </summary>
        public static string GroupBy { get; set; } = "";

        /// <summary>
        /// 是否显示详细信息
        /// </summary>
        public static bool IsDetailedInfoShown { get; set; }

        /// <summary>
        /// 是否显示帮助并退出
        /// </summary>
        public static bool IsHelpShownAndExit { get; set; }

        /// <summary>
        /// 是否是从命令行参数中获取的路径
        /// </summary>
        public static bool IsPathFromArgs { get; set; }

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

            IsDetailedInfoShown = true;
            IsHelpShownAndExit = false;
            IsPathFromArgs = false;
        }

        /// <summary>
        /// 解析命令行参数
        /// </summary>
        /// <param name="arr">参数列表</param>
        /// <exception cref="Exception"></exception>
        public static void parseCommandLineArgs(string[] arr)
        {
            initialize();

            if (arr.Length > 0)
            {
                for (var i = 0; i < arr.Length; i++)
                {
                    if (arr[i].Substring(0, 1) == "-")
                    {
                        var parameter = arr[i].Substring(1);

                        if (parameter == "-Language")
                        {
                            i++;
                            if (i < arr.Length)
                            {
                                Initialize(arr[i]);
                            }
                            else
                            {
                                throw new Exception(GetMessage(MessageKey.ParameterLanguageUsageErrorMissingLanguageCode));
                            }
                            continue;
                        }

                        if (parameter == "-File")
                        {
                            i++;
                            if (i >= arr.Length || !File.Exists(arr[i]) || IsPathFromArgs)
                            {
                                throw new Exception(GetMessage(MessageKey.ParameterErrorMissingPath, parameter.Substring(1)));
                            }

                            IsPathFromArgs = true;
                            FileOrDirectoryPath = arr[i];

                            continue;
                        }

                        if (parameter == "-Directory")
                        {

                            i++;
                            if (i >= arr.Length || !Directory.Exists(arr[i]) || IsPathFromArgs)
                            {
                                throw new Exception(GetMessage(MessageKey.ParameterErrorMissingPath, parameter.Substring(1)));
                            }

                            IsPathFromArgs = true;
                            FileOrDirectoryPath = arr[i];

                            continue;
                        }

                        if (parameter == "-FileKeys")
                        {
                            i++;
                            if (i >= arr.Length || !File.Exists(arr[i]))
                            {
                                throw new Exception(GetMessage(MessageKey.ParameterErrorMissingPath, parameter.Substring(1)));
                            }

                            if (string.IsNullOrEmpty(GroupBy))
                                ComparisonKey = arr[i];

                            continue;
                        }

                        if (parameter == "-Key")
                        {

                            i++;
                            if (i >= arr.Length || Directory.Exists(arr[i]))
                            {
                                throw new Exception(GetMessage(MessageKey.ParameterErrorMissingPath, parameter.Substring(1)));
                            }

                            if (string.IsNullOrEmpty(GroupBy))
                                ComparisonKey = arr[i];

                            continue;
                        }

                        if (parameter == "-Equals")
                        {
                            i++;
                            if (i >= arr.Length || !File.Exists(arr[i]))
                            {
                                throw new Exception(GetMessage(MessageKey.ParameterErrorMissingPath, parameter.Substring(1)));
                            }
                            if (!string.IsNullOrEmpty(GroupBy)) continue;

                            var fileKey = new FileKey(false, false, false, true);
                            ComparisonKey = fileKey.GetFileKeyInfo(arr[i]).Result.Sha256Normalized;

                            continue;
                        }

                        if (parameter == "-GroupBy")
                        {
                            i++;
                            if (i >= arr.Length || arr[i] != "type" && arr[i] != "hash")
                            {
                                throw new Exception(GetMessage(MessageKey.ParameterError, parameter.Substring(1), "<type|hash>"));
                            }

                            if (arr[i] == "type")
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
                            GroupBy = arr[i];

                            continue;
                        }

                        parameter = parameter.ToUpper();
                        if (parameter.Contains("V"))
                        {
                            IsHelpShownAndExit = true;
                            return;
                        }
                        if (parameter.Contains("T"))
                        {
                            parameter = parameter.Replace("T", "");
                            OutTypeOption = true;
                        }
                        if (parameter.Contains("C"))
                        {
                            parameter = parameter.Replace("C", "");
                            OutCrcOption = true;
                        }
                        if (parameter.Contains("M"))
                        {
                            parameter = parameter.Replace("M", "");
                            OutMd5Option = true;
                        }
                        if (parameter.Contains("S"))
                        {
                            parameter = parameter.Replace("S", "");
                            OutSha256Option = true;
                        }
                        if (parameter.Contains("0"))
                        {
                            parameter = parameter.Replace("0", "");
                            IsDetailedInfoShown = false;
                        }

                        if (!string.IsNullOrEmpty(parameter))
                        {
                            throw new Exception(GetMessage(MessageKey.UnrecognizedParameters, parameter));
                        }
                    }
                    else if (string.IsNullOrEmpty(FileOrDirectoryPath))
                    {
                        IsPathFromArgs = true;
                        FileOrDirectoryPath = arr[i];
                    }
                    else if (string.IsNullOrEmpty(ComparisonKey))
                    {
                        if (string.IsNullOrEmpty(GroupBy))
                            ComparisonKey = arr[i];
                    }
                    else
                    {
                        throw new Exception(GetMessage(MessageKey.TooManyParameters, arr[i]));
                    }
                }

            }

            return;
        }

    }
}
