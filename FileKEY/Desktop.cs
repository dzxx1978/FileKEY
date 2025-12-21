using static FileKEY.Language;

namespace FileKEY;

public class Desktop
{
    string[]? fileFullPaths;
    string[]? comparisonKeys;

    private Dictionary<string, FileKeyInfo> fileKeyInfos = new();
    private FileKey fileKey;
    private int beginTop = 0;

    public Desktop()
    {
        fileKey = new(AppStatus.OutTypeOption, AppStatus.OutCrcOption, AppStatus.OutMd5Option, AppStatus.OutSha256Option);
    }

    public async Task GanHuoer()
    {

        if (AppStatus.IsHelpShownAndExit)
        {
            Message.Write(GetHelpShown());
            return;
        }

        if (AppStatus.IsDetailedDisplay)
        {
            if (!string.IsNullOrEmpty(AppStatus.FileOrDirectoryPath))
                Message.WriteLine($"argspath:{AppStatus.FileOrDirectoryPath}");
            if (!string.IsNullOrEmpty(AppStatus.ComparisonKey))
                Message.WriteLine($"argskey:{AppStatus.ComparisonKey}");
        }

        do
        {

            try
            {
                fileFullPaths = getFilePaths();

                if (fileFullPaths is null)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                Message.WarningLine(ex.Message, false);
                continue;
            }

            comparisonKeys = await getComparisonKeys();

            Message.GetPos(out _, out beginTop);
            foreach (var fileFullPath in fileFullPaths)
            {
                var fileKeyInfo = await readFileInfo(fileFullPath);

                if (AppStatus.IsGroup)
                {
                    continue;
                }

                var comparisonInfo = compareChecksums(fileKeyInfo, comparisonKeys);
                if (displayFileInfo(comparisonInfo, fileFullPaths.Length))
                {
                    break;
                }
            }

            displayGroup();

        }
        while (isContinue());

        if (AppStatus.IsDetailedDisplay)
            Message.WriteLine(GetMessage(MessageEnum.End));
    }

    private bool isContinue()
    {

        if (!AppStatus.IsHideMenu && !Console.IsOutputRedirected)
        {
            Message.Attention(GetMessage(MessageEnum.DisplayCompletedPressEnterToContinue));
            Message.Wait("", ConsoleKey.Enter);
            return false;
        }

        return false;
    }

    private async Task<FileKeyInfo> readFileInfo(string fileFullPath, bool addCache = true)
    {

        Task task1 = Task.Run(() => { });
        var tk = new CancellationTokenSource();

        if (AppStatus.IsDetailedDisplay)
        {
            Message.Write(GetMessage(MessageEnum.Wait));
            task1 = Message.WriteLoop(">*", cancellationToken: tk.Token);
        }
        else if (AppStatus.IsGroup && !Console.IsOutputRedirected)
        {
            Message.Write(GetMessage(MessageEnum.Wait), 0, beginTop);
            task1 = Message.WriteLoop(">*", cancellationToken: tk.Token);
            Message.Write($" {fileKeyInfos.Count + (fileKeyInfos.ContainsKey(fileFullPath) ? 0 : 1)} {fileFullPath}");
        }

        FileKeyInfo fileKeyInfo;
        if (fileKeyInfos.ContainsKey(fileFullPath))
        {
            fileKeyInfo = fileKeyInfos[fileFullPath];
        }
        else
        {
            fileKeyInfo = await fileKey.GetFileKeyInfo(fileFullPath, tk.Token);
            if (addCache) fileKeyInfos.Add(fileFullPath, fileKeyInfo);
        }

        tk.Cancel();
        await task1;

        if (AppStatus.IsDetailedDisplay)
            Message.WriteLine($" {GetMessage(MessageEnum.ProcessCompleted)}");

        return fileKeyInfo;
    }

    private void displayGroup()
    {
        if (!AppStatus.IsGroup) return;
        Message.WriteLine("");

        var groups = fileKeyInfos.Values.GroupBy(p => AppStatus.GroupBy == "type" ? p.TypeName : p.Sha256Normalized);
        foreach (var group in groups.Where(p => p.Count() >= AppStatus.GroupMinCount))
        {
            Message.WriteLine($"{group.Key} ({group.Count()})", color: ConsoleColor.Cyan);

            var i = 0;
            foreach (var fileKeyInfo in group)
            {
                i++;
                Message.WriteLine($"  {i}-{Path.GetFullPath(fileKeyInfo.Path)}");
            }

            Message.WriteLine("");
        }
    }

    private bool displayFileInfo(ComparisonInfo comparisonInfo, int fileCount)
    {

        var fileKeyInfo = comparisonInfo.SourceFileKeyInfo!;
        var comparisonKeys = comparisonInfo.ComparisonKeys!;

        if (!fileKeyInfo.Exists)
        {
            Message.WarningLine(fileKeyInfo.ErrMessage, false);
            return false;
        }

        if (AppStatus.IsDetailedDisplay)
        {
            Message.WriteLine($"name:{fileKeyInfo.Name}");
            Message.WriteLine($"time:{fileKeyInfo.Time.ToString("yyyy-MM-dd HH:mm:ss")}");
            Message.WriteLine($"size:{fileKeyInfo.DisplaySize}({fileKeyInfo.Length}B)");
        }

        if (AppStatus.IsDetailedDisplay || string.IsNullOrEmpty(AppStatus.ComparisonKey))
        {
            if (AppStatus.OutTypeOption)
                Message.WriteLine($"{Language.Type}:{fileKeyInfo.TypeName}");
            if (AppStatus.OutCrcOption)
                Message.WriteLine($"{Language.Crc}:{fileKeyInfo.Crc32Normalized}");
            if (AppStatus.OutMd5Option)
                Message.WriteLine($"{Language.Md5}:{fileKeyInfo.Md5Normalized}");
            if (AppStatus.OutSha256Option)
                Message.WriteLine($"{Language.Sha256}:{fileKeyInfo.Sha256Normalized}");
        }

        if (comparisonInfo.IsEqual)
        {
            Message.WriteLine(comparisonInfo.ToString(), color: ConsoleColor.Green);
            if (fileCount > 1 && comparisonKeys.Length == 1)
            {
                if (!AppStatus.IsDetailedDisplay)
                {
                    Message.WriteLine(Path.GetFullPath(fileKeyInfo.Path));
                }
                return true;
            }
        }
        else if (fileCount == 1 || comparisonKeys.Length > 1)
        {
            Message.WarningLine(comparisonInfo.ToString(), false);
        }

        return false;
    }

    private ComparisonInfo compareChecksums(FileKeyInfo fileKeyInfo, string[] comparisonKeys)
    {
        var comparisonInfo = new ComparisonInfo();
        comparisonInfo.SourceFileKeyInfo = fileKeyInfo;
        comparisonInfo.ComparisonKeys = comparisonKeys;

        if (!fileKeyInfo.Exists || comparisonKeys.Length == 0) return comparisonInfo;

        var crc = fileKeyInfo.Crc32Normalized;
        var md5 = fileKeyInfo.Md5Normalized;
        var sha256 = fileKeyInfo.Sha256Normalized;

        var index = 0;

        var isComparisonKeyFile = comparisonKeys.Length > 1;
        foreach (var lineKey in comparisonKeys)
        {
            index++;
            comparisonInfo.InKeyColumn = 0;

            if (AppStatus.OutCrcOption && (isComparisonKeyFile && lineKey.Contains(crc) || lineKey == crc))
            {
                comparisonInfo.EqualityHashValue = crc;
                comparisonInfo.InKeyColumn = lineKey.IndexOf(crc) + 1;
                comparisonInfo.IsCrc32Equal = true;
            }
            else if (AppStatus.OutMd5Option && (isComparisonKeyFile && lineKey.Contains(md5) || lineKey == md5))
            {
                comparisonInfo.EqualityHashValue = md5;
                comparisonInfo.InKeyColumn = lineKey.IndexOf(md5) + 1;
                comparisonInfo.IsMd5Equal = true;
            }
            else if (AppStatus.OutSha256Option && (isComparisonKeyFile && lineKey.Contains(sha256) || lineKey == sha256))
            {
                comparisonInfo.EqualityHashValue = sha256;
                comparisonInfo.InKeyColumn = lineKey.IndexOf(sha256) + 1;
                comparisonInfo.IsSha256Equal = true;
            }

            if (comparisonInfo.IsEqual)
            {
                comparisonInfo.InKeyRow = index;
                break;
            }
        }

        return comparisonInfo;

    }

    private string[]? getFilePaths()
    {
        var isFile = AppStatus.IsFile;
        var isDirectory = AppStatus.IsDirectory;
        var fileOrDirectoryPath = isFile || isDirectory || AppStatus.IsGroup
            ? AppStatus.FileOrDirectoryPath
            : Message.ReadPath(GetMessage(MessageEnum.PleaseEnterTheFilePath), string.Empty);

        if (!isFile && !isDirectory)
        {
            AppStatus.SetFileOrDirectoryPath(fileOrDirectoryPath, setHideMenu: false);
            isFile = AppStatus.IsFile;
            isDirectory = AppStatus.IsDirectory;
        }

        AppStatus.SetFileOrDirectoryPath(string.Empty);
        if (fileOrDirectoryPath.ToLower() == "exit")
        {
            return null;
        }

        var resultFilePaths = new List<string>();
        if (isDirectory)
        {
            return FileKey.GetSubDirectoryFiles(fileOrDirectoryPath, AppStatus.SubDirectory).ToArray();
        }
        else if (isFile)
        {
            return [Path.GetFullPath(fileOrDirectoryPath)];
        }
        else
        {
            throw new Exception(GetMessage(MessageEnum.TheInputFilePathDoesNotExist, fileOrDirectoryPath));
        }

    }

    private async Task<string[]> getComparisonKeys()
    {
        var comparisonKeys = new List<string>();

        if (AppStatus.IsGroup)
            return comparisonKeys.ToArray();

        var comparisonKey = "";
        if (!AppStatus.IsHideMenu)
        {
            comparisonKey = Message.ReadString(GetMessage(MessageEnum.PleaseEnterTheVerificationKeyValueOrTheFilePathWhereTheKeyValueIsStored), true);
            if (!AppStatus.SetComparisonKey(comparisonKey))
            {
                return comparisonKeys.ToArray();
            }
        }
        comparisonKey = AppStatus.ComparisonKey;

        if (AppStatus.IsTxtFileKeys)
        {
            comparisonKeys =  ConfigFile.LoadConfigFile(comparisonKey).ToList();
        }
        else if (AppStatus.IsEqualsFile)
        {
            comparisonKeys.Add((await readFileInfo(comparisonKey, false)).Sha256Normalized);
        }
        else if (AppStatus.IsStringKey)
        {
            comparisonKeys.Add(comparisonKey);
        }

        return comparisonKeys.ToArray();
    }

}
