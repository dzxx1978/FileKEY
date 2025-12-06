using FileKEY;
using System.Reflection;

var fileOrDirectoryPath = "";
var comparisonKey = "";

var outTypeOption = 1;
var outCrcOption = 1;
var outMd5Option = 1;
var outSha256Option = 1;

var isDetailedInfoShown = true;
var isHelpShownAndExit = false;

var isPathFromArgs = false;

try
{
    Message.LoadLanguage();
    parseCommandLineArgs();
}
catch (Exception ex)
{
    isHelpShownAndExit = true;
    isDetailedInfoShown = true;
    Message.WarningLine(ex.Message, false);
}

if (isHelpShownAndExit)
{
    Message.WriteLine("https://github.com/dzxx1978/FileKEY");
    Message.WriteLine($"FileKEY {Assembly.GetExecutingAssembly().GetName().Version?.ToString()} by zxx 2025");
    Message.WriteLine("FileKEY [path] [key] [-0tcms]");
    Message.WriteLine(" -0 small print");
    Message.WriteLine(" -t only type");
    Message.WriteLine(" -c only crc");
    Message.WriteLine(" -m only md5");
    Message.WriteLine(" -s only sha256");
    return;
}

if (isDetailedInfoShown)
{
    if (!string.IsNullOrEmpty(fileOrDirectoryPath))
        Message.WriteLine($"argspath:{fileOrDirectoryPath}");
    if (!string.IsNullOrEmpty(comparisonKey))
        Message.WriteLine($"argskey:{comparisonKey}");
}

var fileKey = new FileKey(outTypeOption == 1, outCrcOption == 1, outMd5Option == 1, outSha256Option == 1);

await GanHuoer();
return;

async Task GanHuoer()
{
    do
    {
        var filePaths = new List<string>();

        try
        {
            filePaths = getPaths();

            if (filePaths is null)
            {
                break;
            }
        }
        catch (Exception ex)
        {
            Message.WarningLine(ex.Message, false);
            continue;
        }

        getComparisonKey();

        foreach (var file in filePaths)
        {
            var fileKeyInfo = await readFileInfo(file);

            if (fileKeyInfo.Exists == true)
            {
                displayFileInfoDetails(fileKeyInfo);
                compareChecksums(fileKeyInfo);
            }
            else
            {
                Message.WarningLine(fileKeyInfo.ErrMessage, false);
            }

        }

    }
    while (isContinue());

    if (isDetailedInfoShown)
        Message.WriteLine("END");

}

bool isContinue() {

    if (!isPathFromArgs && !Console.IsOutputRedirected)
    {
        Message.Attention("显示完毕，按回车继续下一个。");
        Message.Wait("", ConsoleKey.Enter);
        return true;
    }

    return false;
}

async Task<FileKeyInfo> readFileInfo(string file)
{

    Task task1 = Task.Run(() => { });
    var tk = new CancellationTokenSource();

    if (isDetailedInfoShown)
    {
        Message.Write("稍等 >>");
        task1 = Message.WriteLoop(">*", cancellationToken: tk.Token);
    }

    var key = await fileKey.GetFileKeyInfo(file, tk.Token);

    tk.Cancel();
    await task1;

    if(isDetailedInfoShown)
        Message.WriteLine($" 处理完毕");

    return key;
}

void displayFileInfoDetails(FileKeyInfo fileKeyInfo) {

    if (isDetailedInfoShown)
    {
        Message.WriteLine($"name:{fileKeyInfo.Name}");
        Message.WriteLine($"time:{fileKeyInfo.Time.ToString("yyyy-MM-dd HH:mm:ss")}");
        Message.WriteLine($"size:{fileKeyInfo.DisplaySize}({fileKeyInfo.Length}B)");

        if (outTypeOption == 1)
            Message.WriteLine($"type:{fileKeyInfo.TypeName}");
        if (outCrcOption == 1)
            Message.WriteLine($"crc:{fileKeyInfo.Crc32Normalized}");
        if (outMd5Option == 1)
            Message.WriteLine($"md5:{fileKeyInfo.Md5Normalized}");
        if (outSha256Option == 1)
            Message.WriteLine($"sha256:{fileKeyInfo.Sha256Normalized}");
    }

}

bool compareChecksums(FileKeyInfo fileKeyInfo)
{

    if (string.IsNullOrEmpty(comparisonKey) == false)
    {
        var comparisonKeyLines = new List<string>();
        var isComparisonKeyFile = false;
        if (File.Exists(comparisonKey))
        {
            isComparisonKeyFile = true;
            using (StreamReader reader = new StreamReader(comparisonKey))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    comparisonKeyLines.Add(line);
                }
            }
        }
        else
        {
            comparisonKeyLines.Add(comparisonKey);
        }

        var crc = fileKeyInfo.Crc32Normalized;
        var md5 = fileKeyInfo.Md5Normalized;
        var sha256 = fileKeyInfo.Sha256Normalized;

        var matchedHashType = "";
        var matchedRowIndex = 0;
        var matchedColumnIndex = 0;

        foreach (var lineKey in comparisonKeyLines)
        {
            matchedRowIndex++;
            matchedColumnIndex = 0;

            if (outCrcOption == 1 && (isComparisonKeyFile && lineKey.Contains(crc) || lineKey == crc))
            {
                matchedHashType = "CRC";
                matchedColumnIndex = lineKey.IndexOf(crc) + 1;
            }
            else if (outMd5Option == 1 && (isComparisonKeyFile && lineKey.Contains(md5) || lineKey == md5))
            {
                matchedHashType = "MD5";
                matchedColumnIndex = lineKey.IndexOf(md5) + 1;
            }
            else if (outSha256Option == 1 && (isComparisonKeyFile && lineKey.Contains(sha256) || lineKey == sha256))
            {
                matchedHashType = "sha256";
                matchedColumnIndex = lineKey.IndexOf(sha256) + 1;
            }

            if (matchedColumnIndex > 0)
            {
                if (isComparisonKeyFile)
                {
                    Message.WriteLine($"matched-{matchedHashType}-{matchedRowIndex}.{matchedColumnIndex}", color: ConsoleColor.Green);
                }
                else
                {
                    Message.WriteLine($"matched-{matchedHashType}", color: ConsoleColor.Green);
                }
                return true;
            }
        }

        var outKey = outCrcOption == 1 && outMd5Option == 0 && outSha256Option == 0 ? crc
            : outCrcOption == 0 && outMd5Option == 1 && outSha256Option == 0 ? md5
            : outCrcOption == 0 && outMd5Option == 0 && outSha256Option == 1 ? sha256
            : !isComparisonKeyFile && outCrcOption == 1 && crc.Length == comparisonKey.Length ? crc
            : !isComparisonKeyFile && outMd5Option == 1 && md5.Length == comparisonKey.Length ? md5
            : !isComparisonKeyFile && outSha256Option == 1 && sha256.Length == comparisonKey.Length ? sha256
            : isComparisonKeyFile ? $"NoKeyInFile({Path.GetFileName(comparisonKey)})"
            : $"NoKeyTheLengthIs({comparisonKey.Length})";

        Message.WarningLine($"miss-{outKey}", false);

    }

    return false;

}

List<string>? getPaths()
{

    if (!isPathFromArgs)
        fileOrDirectoryPath = Message.ReadPath("请输入文件路径", fileOrDirectoryPath);

    if (string.IsNullOrEmpty(fileOrDirectoryPath) || fileOrDirectoryPath.ToLower() == "exit")
    {
        return null;
    }

    var resultFilePaths = new List<string>();
    if (Directory.Exists(fileOrDirectoryPath))
    {
        resultFilePaths = Directory.GetFiles(fileOrDirectoryPath).ToList();
    }
    else if (File.Exists(fileOrDirectoryPath))
    {
        resultFilePaths.Add(fileOrDirectoryPath);
    }
    else
    {
        throw new Exception($"输入的文件路径{fileOrDirectoryPath}不存在");
    }

    return resultFilePaths;
}

void getComparisonKey()
{

    if (!isPathFromArgs)
        comparisonKey = Message.ReadString("请输入验证Key值或存储Key值的文件路径", true);

}

bool parseCommandLineArgs() {

    var arr = args.ToArray();
    if (arr.Length > 0)
    {
        for (var i = 0; i < arr.Length; i++)
        {
            if (arr[i].Substring(0, 1) == "-")
            {
                var parameter = arr[i].Substring(1).ToUpper();

                if (parameter.Contains("V"))
                {
                    isHelpShownAndExit = true;
                    return true;
                }
                if (parameter.Contains("T"))
                {
                    parameter = parameter.Replace("T", "");
                    outTypeOption = 2;
                }
                if (parameter.Contains("C"))
                {
                    parameter = parameter.Replace("C", "");
                    outCrcOption = 2;
                }
                if (parameter.Contains("M"))
                {
                    parameter = parameter.Replace("M", "");
                    outMd5Option = 2;
                }
                if (parameter.Contains("S"))
                {
                    parameter = parameter.Replace("S", "");
                    outSha256Option = 2;
                }
                if (parameter.Contains("0"))
                {
                    parameter = parameter.Replace("0", "");
                    isDetailedInfoShown = false;
                }

                if (!string.IsNullOrEmpty(parameter))
                {
                    throw new Exception($"无法识别的参数：-{parameter}");
                }
            }
            else if (string.IsNullOrEmpty(fileOrDirectoryPath))
            {
                isPathFromArgs = true;
                fileOrDirectoryPath = arr[i];
            }
            else if (string.IsNullOrEmpty(comparisonKey))
            {
                comparisonKey = arr[i];
            }
            else
            {
                throw new Exception($"参数过多：{arr[i]}");
            }
        }

        if (outTypeOption == 2 || outCrcOption == 2 || outMd5Option == 2 || outSha256Option == 2)
        {
            outTypeOption--;
            outCrcOption--;
            outMd5Option--;
            outSha256Option--;
        }

        return true;
    }
     
    return false;
}