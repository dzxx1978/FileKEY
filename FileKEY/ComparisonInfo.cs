using System.Security.Cryptography;
using static FileKEY.Language;

namespace FileKEY
{
    public class ComparisonInfo
    {
        public FileKeyInfo? SourceFileKeyInfo { get; set; } = null;
        public string[]? ComparisonKeys { get; set; } = null;

        public bool IsCrc32Equal { get; set; } = false;
        public bool IsMd5Equal { get; set; } = false;
        public bool IsSha256Equal { get; set; } = false;

        public long InKeyColumn { get; set; } = 0;
        public long InKeyRow { get; set; } = 0;

        public string EqualityHashValue { get; set; } = "";

        public bool IsEqual => IsCrc32Equal || IsMd5Equal || IsSha256Equal;

        public string EqualityType
        {
            get
            {
                if (IsSha256Equal)
                    return Language.Sha256;
                else if (IsMd5Equal)
                    return Language.Md5;
                else if (IsCrc32Equal)
                    return Language.Crc;
                else
                    return "None";
            }
        }

        public override string ToString()
        {
            if (SourceFileKeyInfo is null || !SourceFileKeyInfo.Exists || ComparisonKeys is null || ComparisonKeys.Length == 0) return "";

            var outKey = "";
            var isComparisonKeyFile = ComparisonKeys.Length > 1;

            if (IsEqual)
            {
                if (isComparisonKeyFile)
                {
                    outKey = GetMessage(MessageKey.MatchedInKeysFile, EqualityType, EqualityHashValue, InKeyRow, InKeyColumn);
                }
                else
                {
                    outKey = GetMessage(MessageKey.Matched, EqualityType, EqualityHashValue);
                }
            }
            else
            {
                outKey = AppOption.OnlyOutHashOption(nameof(AppOption.OutCrcOption)) ? SourceFileKeyInfo.Crc32Normalized
                       : AppOption.OnlyOutHashOption(nameof(AppOption.OutMd5Option)) ? SourceFileKeyInfo.Md5Normalized
                       : AppOption.OnlyOutHashOption(nameof(AppOption.OutSha256Option)) ? SourceFileKeyInfo.Sha256Normalized
                       : !isComparisonKeyFile && AppOption.OutCrcOption && SourceFileKeyInfo.Crc32Normalized.Length == ComparisonKeys[0].Length ? SourceFileKeyInfo.Crc32Normalized
                       : !isComparisonKeyFile && AppOption.OutMd5Option && SourceFileKeyInfo.Md5Normalized.Length == ComparisonKeys[0].Length ? SourceFileKeyInfo.Md5Normalized
                       : !isComparisonKeyFile && AppOption.OutSha256Option && SourceFileKeyInfo.Sha256Normalized.Length == ComparisonKeys[0].Length ? SourceFileKeyInfo.Sha256Normalized
                       : isComparisonKeyFile ? GetMessage(MessageKey.NoKeyInFile, Path.GetFileName(AppOption.ComparisonKey))
                       : GetMessage(MessageKey.NoKeyTheLengthIs, ComparisonKeys[0].Length);

                outKey = GetMessage(MessageKey.Miss, outKey);
            }

            return outKey;

        }
    }
}