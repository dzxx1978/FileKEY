
namespace FileKEY
{
    public static class AppOption
    {
        private static int _outTypeOption = 1;
        private static int _outCrcOption = 1;
        private static int _outMd5Option = 1;
        private static int _outSha256Option = 1;

        private static int _outMaxOption { get => _outTypeOption == 2 || _outCrcOption == 2 || _outMd5Option == 2 || _outSha256Option == 2 ? 1 : 0; }

        /// <summary>
        /// 是否仅有指定的哈希输出选项被启用
        /// </summary>
        /// <param name="hashName">哈希选项名称</param>
        /// <returns></returns>
        public static bool OnlyOutHashOption(string hashName) {

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
        public static bool OutTypeOption { 
            get => _outTypeOption > _outMaxOption;
            set => _outTypeOption = value ? 2 : 0;
        }

        /// <summary>
        /// 是否计算并输出CRC32哈希值
        /// </summary>
        public static bool OutCrcOption { 
            get => _outCrcOption > _outMaxOption;
            set => _outCrcOption = value ? 2 : 0;
        }

        /// <summary>
        /// 是否计算并输出MD5哈希值
        /// </summary>
        public static bool OutMd5Option { 
            get => _outMd5Option > _outMaxOption;
            set => _outMd5Option = value ? 2 : 0;
        }

        /// <summary>
        /// 是否计算并输出SHA256哈希值
        /// </summary>
        public static bool OutSha256Option { 
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
        /// 是否显示详细信息
        /// </summary>
        public static bool IsDetailedInfoShown { get; set; } = true;

        /// <summary>
        /// 是否显示帮助并退出
        /// </summary>
        public static bool IsHelpShownAndExit { get; set; } = false;

        /// <summary>
        /// 是否是从命令行参数中获取的路径
        /// </summary>
        public static bool IsPathFromArgs { get; set; } = false;

    }
}
