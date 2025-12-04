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
    public string Type { get; set; } = "";

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
    public uint CRC { get; set; }

    /// <summary>
    /// md5校验值
    /// </summary>
    public string MD5 { get; set; } = "";

    /// <summary>
    /// sha256校验值
    /// </summary>
    public string Sha256 { get; set; } = "";

    public decimal GB
    {
        get
        {
            return MB / 1024;
        }
    }
    public decimal MB
    {
        get
        {
            return KB / 1024;
        }
    }
    public decimal KB
    {
        get
        {
            return Length / 1024;
        }
    }

    public string Size
    {
        get {

            return GB > 1 ? $"{Math.Round(GB, 2)}G" : MB > 1 ? $"{Math.Round(MB, 2)}M" : $"{Math.Round(KB, 2)}K";
        
        }
    
    }
}