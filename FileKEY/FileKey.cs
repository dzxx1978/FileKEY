using System.Security.Cryptography;
using System.Text;

namespace FileKEY;

public class FileKey
{
    private static uint[] Crc32Table = new uint[256];
    private static Dictionary<string, string> fileTypes = new();

    public FileKey()
    {
        if (fileTypes.Count == 0)
        {
            for (uint i = 0; i < 256; i++)
            {
                uint crc = i;
                for (int j = 0; j < 8; j++)
                {
                    crc = (crc & 1) != 0 ? (crc >> 1) ^ 0xEDB88320 : crc >> 1;
                }
                Crc32Table[i] = crc;
            }

            fileTypes.Add("00000000000000000", "Iso(iso)");
            fileTypes.Add("%PDF-", "PDF(pdf)");
            fileTypes.Add("255216255224016JFIF", "JPEG(jpg)");
            fileTypes.Add("137PNG", "PNG(png)");
            fileTypes.Add("PK3410", "Office(xlsx,docx)");
            fileTypes.Add("PK3420", "Zip(zip)");
            fileTypes.Add("2082071722416117726225", "Office(xls,doc)");
            fileTypes.Add("Rar!", "Rar(rar)");
            fileTypes.Add("x218c", "Image(dmg)");
            fileTypes.Add("MZ", "WindowsApp(exe,dll)");
            fileTypes.Add("BSJB", "DebugDatabase(pdb)");
            fileTypes.Add("KDMV", "VmwareDisk(vmdk)");
        }

    }

    public async Task<FileKeyInfo> GetFileKeyInfo(string filePath, string outPut, CancellationToken cancellationToken = default)
    {
        var key = new FileKeyInfo();
        key.Path = filePath;

        var fileInfo = new FileInfo(filePath);
        if (fileInfo.Exists == false)
        {
            key.ErrMessage = "没有这个文件！";
            key.Exists = false;
            return key;
        }

        key.Name = fileInfo.Name;
        key.Length = fileInfo.Length;
        key.Time = fileInfo.LastWriteTime;

        try
        {

            //type
            if (outPut.Substring(0, 1) == "1")
                key.Type = await GetFileType(filePath, cancellationToken);

            //CRC
            if (outPut.Substring(1, 1) == "1")
                key.CRC = await GetFileCRC(filePath, cancellationToken);

            //md5
            if (outPut.Substring(2, 1) == "1")
                key.MD5 = await GetFileMD5(filePath, cancellationToken);

            //sha
            if (outPut.Substring(3, 1) == "1")
                key.Sha256 = await GetFileSha256(filePath, cancellationToken);

        }
        catch (Exception ex)
        {
            key.ErrMessage = ex.Message;
            key.Exists = false;
        }

        return key;
    }

    public async Task<string> GetFileMD5(string filePath, CancellationToken cancellationToken = default)
    {
        using var fileStream = File.OpenRead(filePath);
        using var md5 = MD5.Create();
        return BitConverter.ToString(await md5.ComputeHashAsync(fileStream, cancellationToken));
    }

    public async Task<string> GetFileSha256(string filePath, CancellationToken cancellationToken = default)
    {
        using var fileStream = File.OpenRead(filePath);
        using var sha256 = SHA256.Create();
        return BitConverter.ToString(await sha256.ComputeHashAsync(fileStream, cancellationToken));
    }

    public async Task<uint> GetFileCRC(string filePath, CancellationToken cancellationToken = default)
    {
        uint crc = 0xFFFFFFFF;
        var buffer = new byte[8192];

        using var fileStream = File.OpenRead(filePath);

        int bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length);

        while (bytesRead > 0)
        {
            for (int i = 0; i < bytesRead; i++)
            {
                crc = (crc >> 8) ^ Crc32Table[(crc ^ buffer[i]) & 0xFF];
            }

            bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length);
        }

        return ~crc;

    }

    private async Task<string> GetFileType(string filePath, CancellationToken cancellationToken = default)
    {
        var buffer = new byte[1024];
        using var fileStream = File.OpenRead(filePath);
        int bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length);

        if (bytesRead == 0)
        {
            return "null;";
        }

        var fileType = new StringBuilder();

        if (buffer.SequenceEqual(Encoding.UTF8.GetPreamble()))
        {
            fileType.Append($"UTF8(txt)");
        }
        else if (buffer.SequenceEqual(Encoding.Unicode.GetPreamble()))
        {
            fileType.Append($"Unicode(txt)");
        }
        else if (buffer.SequenceEqual(Encoding.BigEndianUnicode.GetPreamble()))
        {
            fileType.Append($"BigEndianUnicode(txt)");
        }
        else
        {
            var fileClass = new StringBuilder();
            var ifBinary = false;
            for (var i = 0; i < bytesRead; i++)
            {
                var num = buffer[i];
                if (num > 0 && num < 32 && num != 9 && num != 10 && num != 13)
                {
                    ifBinary = true;
                }

                if (i > 15 || ifBinary)
                {
                    if (ifBinary) break;
                }
                else
                {
                    var ch = Convert.ToChar(num);
                    if ("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789`~!@#$%^&*()-=_+[]{}|;':,./<>?\\\"".Contains(ch))
                    {
                        fileClass.Append(ch);
                    }
                    else
                    {
                        fileClass.Append(num);
                    }
                    if (fileTypes.ContainsKey(fileClass.ToString()))
                    {
                        fileType.Append($"{fileTypes[fileClass.ToString()]}; ");
                    }
                }
            }
            if (ifBinary)
            {
                fileType.Append(fileClass);
            }
            else
            {
                fileType.Append("Text(txt);");
            }
        }

        return fileType.ToString();

    }

}
