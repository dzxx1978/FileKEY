using Microsoft.VisualBasic.FileIO;
using System.Security.Cryptography;
using System.Text;

namespace FileKEY;

public class FileKey
{
    private static bool isCrc32TableInitialized = false;
    private static uint[] Crc32Table = new uint[256];
    private static void InitializeCrc32Table()
    {
        if (!isCrc32TableInitialized)
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
            isCrc32TableInitialized = true;
        }
    }

    private static bool isFileTypesInitialized = false;
    private static Dictionary<string, string> fileTypes = new();
    private static void InitializeFileTypes()
    {
        if (!isFileTypesInitialized)
        {
            fileTypes.Add("0000000000000000", "Iso(iso)");//type:4552806260000000000
            fileTypes.Add("255044462D312E", "PDF(pdf)");//type:255044462D312E37AA342030206F62
            fileTypes.Add("FFD8FFE00104A464946", "JPEG(jpg)");//type:FFD8FFE00104A464946011001
            fileTypes.Add("89504E47", "PNG(png)");//type:89504E47DA1AA000D49484452
            fileTypes.Add("504B34A0", "Office(xlsx,docx)");//type:504B34A00000874EE24000
            fileTypes.Add("504B3414", "Zip(zip)");         //type:504B341400080AA596D5B596D
            fileTypes.Add("D0CF11E0A1B11AE1", "Office(xls,doc); WindowsInstaller(msi)");//type:D0CF11E0A1B11AE100000000
            fileTypes.Add("52617221", "Rar(rar)");//type: 526172211A710F844A420C158
            fileTypes.Add("78DA63", "Image(dmg)");//type:78DA63601854318FCFBFFFF1D10332
            fileTypes.Add("4D5A", "App(exe,dll)");//type:4D5A90030004000FFFF00
            fileTypes.Add("4B444D56", "VmwareDisk(vmdk)");//type:4B444D561000300000A0 type:4B444D5610003000007F0
            isFileTypesInitialized = true;
        }
    }

    public async Task<FileKeyInfo> GetFileKeyInfo(string filePath, string outPutControl, CancellationToken cancellationToken = default)
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
            if (outPutControl.Substring(0, 1) == "1")
                key.TypeName = await GetFileType(filePath, cancellationToken);

            //CRC
            if (outPutControl.Substring(1, 1) == "1")
                key.Crc32Hash = await GetFileCRC(filePath, cancellationToken);

            //md5
            if (outPutControl.Substring(2, 1) == "1")
                key.Md5Hash = await GetFileMD5(filePath, cancellationToken);

            //sha
            if (outPutControl.Substring(3, 1) == "1")
                key.Sha256Hash = await GetFileSha256(filePath, cancellationToken);
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
        InitializeCrc32Table();

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

    public async Task<string> GetFileType(string filePath, CancellationToken cancellationToken = default)
    {
        InitializeFileTypes();

        var buffer = new byte[16];
        using var fileStream = File.OpenRead(filePath);
        int bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length);

        if (bytesRead == 0)
        {
            return "null;";
        }

        var fileType = new StringBuilder();
        var fileBytes = new StringBuilder();

        for (var i = 0; i < bytesRead; i++)
        {
            fileBytes.Append(buffer[i].ToString("X"));
            if (fileTypes.ContainsKey(fileBytes.ToString()))
            {
                fileType.Append($"{fileTypes[fileBytes.ToString()]}; ");
            }
        }

        if (fileType.Length==0)
            fileType.Append(fileBytes);

        return fileType.ToString();

    }

}
