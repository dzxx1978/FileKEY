namespace FileKEY;

public class FileKeyInfo
{
    /// <summary>
    /// 文件是否存在
    /// </summary>
    public bool Exists { get; set; } = true;

    /// <summary>
    /// 错误信息
    /// </summary>
    public string ErrMessage { get; set; } = "";

    /// <summary>
    /// 文件路径
    /// </summary>
    public string Path { get; set; } = "";

    /// <summary>
    /// 文件名
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// 文件类型
    /// </summary>
    public string TypeName { get; set; } = "";

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime Time { get; set; }

    /// <summary>
    /// 文件大小
    /// </summary>
    public long Length { get; set; }

    /// <summary>
    /// crc校验值
    /// </summary>
    public uint Crc32Hash { get; set; }

    /// <summary>
    /// md5校验值
    /// </summary>
    public string Md5Hash { get; set; } = "";

    /// <summary>
    /// sha256校验值
    /// </summary>
    public string Sha256Hash { get; set; } = "";

    public decimal GB => MB / 1024;

    public decimal MB => KB / 1024;

    public decimal KB => Length / 1024;

    public string DisplaySize => GB > 1 ? $"{Math.Round(GB, 2)}G" : MB > 1 ? $"{Math.Round(MB, 2)}M" : $"{Math.Round(KB, 2)}K";

    public string Crc32Normalized => Crc32Hash.ToString("X8");

    public string Md5Normalized => Md5Hash.Replace("-", "").ToLower();

    public string Sha256Normalized => Sha256Hash.Replace("-", "").ToLower();

    public ComparisonInfo ComparisonInfo { get; set; } = new();

}