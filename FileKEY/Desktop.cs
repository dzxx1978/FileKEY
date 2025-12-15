using static FileKEY.Language;

namespace FileKEY
{
    public class Desktop
    {
        string[]? fileFullPaths;
        string[]? comparisonKeys;

        private Dictionary<string, FileKeyInfo> fileKeyInfos = new();
        private FileKey fileKey;
        private int beginTop = 0;

        public Desktop()
        {
            fileKey = new(AppOption.OutTypeOption, AppOption.OutCrcOption, AppOption.OutMd5Option, AppOption.OutSha256Option);
        }

        public async Task GanHuoer()
        {

            if (AppOption.IsHelpShownAndExit)
            {
                Message.Write(GetHelpShown());
                return;
            }

            if (AppOption.IsDetailedInfoShown)
            {
                if (!string.IsNullOrEmpty(AppOption.FileOrDirectoryPath))
                    Message.WriteLine($"argspath:{AppOption.FileOrDirectoryPath}");
                if (!string.IsNullOrEmpty(AppOption.ComparisonKey))
                    Message.WriteLine($"argskey:{AppOption.ComparisonKey}");
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

                    if (AppOption.IsGroup)
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

            if (AppOption.IsDetailedInfoShown)
                Message.WriteLine(GetMessage(MessageKey.End));

        }

        private bool isContinue()
        {

            if (!AppOption.IsPathFromArgs && !Console.IsOutputRedirected)
            {
                Message.Attention(GetMessage(MessageKey.DisplayCompletedPressEnterToContinue));
                Message.Wait("", ConsoleKey.Enter);
                return true;
            }

            return false;
        }

        private async Task<FileKeyInfo> readFileInfo(string fileFullPath)
        {

            Task task1 = Task.Run(() => { });
            var tk = new CancellationTokenSource();

            if (AppOption.IsDetailedInfoShown)
            {
                Message.Write(GetMessage(MessageKey.Wait));
                task1 = Message.WriteLoop(">*", cancellationToken: tk.Token);
            }
            else if (AppOption.IsGroup && !Console.IsOutputRedirected)
            {
                Message.Write(GetMessage(MessageKey.Wait), 0, beginTop);
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
                fileKeyInfos.Add(fileFullPath, fileKeyInfo);
            }

            tk.Cancel();
            await task1;

            if (AppOption.IsDetailedInfoShown)
                Message.WriteLine(GetMessage(MessageKey.ProcessCompleted));

            return fileKeyInfo;
        }

        private void displayGroup()
        {
            if (!AppOption.IsGroup) return;
            Message.WriteLine("");

            var groups = fileKeyInfos.Values.GroupBy(p => AppOption.GroupBy == "type" ? p.TypeName : p.Sha256Normalized);
            foreach (var group in groups.Where(p => p.Count() >= AppOption.GroupMinCount))
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

            if (AppOption.IsDetailedInfoShown)
            {
                Message.WriteLine($"name:{fileKeyInfo.Name}");
                Message.WriteLine($"time:{fileKeyInfo.Time.ToString("yyyy-MM-dd HH:mm:ss")}");
                Message.WriteLine($"size:{fileKeyInfo.DisplaySize}({fileKeyInfo.Length}B)");
            }

            if (AppOption.IsDetailedInfoShown || string.IsNullOrEmpty(AppOption.ComparisonKey))
            {
                if (AppOption.OutTypeOption)
                    Message.WriteLine($"{Language.Type}:{fileKeyInfo.TypeName}");
                if (AppOption.OutCrcOption)
                    Message.WriteLine($"{Language.Crc}:{fileKeyInfo.Crc32Normalized}");
                if (AppOption.OutMd5Option)
                    Message.WriteLine($"{Language.Md5}:{fileKeyInfo.Md5Normalized}");
                if (AppOption.OutSha256Option)
                    Message.WriteLine($"{Language.Sha256}:{fileKeyInfo.Sha256Normalized}");
            }

            if (comparisonInfo.IsEqual)
            {
                Message.WriteLine(comparisonInfo.ToString(), color: ConsoleColor.Green);
                if (fileCount > 1 && comparisonKeys.Length == 1)
                {
                    if (!AppOption.IsDetailedInfoShown)
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

                if (AppOption.OutCrcOption && (isComparisonKeyFile && lineKey.Contains(crc) || lineKey == crc))
                {
                    comparisonInfo.EqualityHashValue = crc;
                    comparisonInfo.InKeyColumn = lineKey.IndexOf(crc) + 1;
                    comparisonInfo.IsCrc32Equal = true;
                }
                else if (AppOption.OutMd5Option && (isComparisonKeyFile && lineKey.Contains(md5) || lineKey == md5))
                {
                    comparisonInfo.EqualityHashValue = md5;
                    comparisonInfo.InKeyColumn = lineKey.IndexOf(md5) + 1;
                    comparisonInfo.IsMd5Equal = true;
                }
                else if (AppOption.OutSha256Option && (isComparisonKeyFile && lineKey.Contains(sha256) || lineKey == sha256))
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

            var fileOrDirectoryPath = string.IsNullOrEmpty(AppOption.FileOrDirectoryPath)
                ? Message.ReadPath(GetMessage(MessageKey.PleaseEnterTheFilePath), string.Empty)
                : AppOption.FileOrDirectoryPath;

            AppOption.FileOrDirectoryPath = string.Empty;

            if (fileOrDirectoryPath.ToLower() == "exit")
            {
                return null;
            }

            var resultFilePaths = new List<string>();
            if (Directory.Exists(fileOrDirectoryPath))
            {
                return getSubDirectoryFiles(fileOrDirectoryPath, AppOption.SubDirectory).ToArray();
            }
            else if (File.Exists(fileOrDirectoryPath))
            {
                return [Path.GetFullPath(fileOrDirectoryPath)];
            }
            else
            {
                throw new Exception(GetMessage(MessageKey.TheInputFilePathDoesNotExist, fileOrDirectoryPath));
            }

        }

        private List<string> getSubDirectoryFiles(string directoryPath, int subCount)
        {
            var resultFilePaths = new List<string>();
            resultFilePaths.AddRange(Directory.GetFiles(directoryPath));
            if (subCount > 0)
            {
                subCount--;
                var subDirectories = Directory.GetDirectories(directoryPath);
                foreach (var subDirectory in subDirectories)
                {
                    resultFilePaths.AddRange(getSubDirectoryFiles(subDirectory, subCount));
                }
            }
            return resultFilePaths;
        }

        private async Task<string[]> getComparisonKeys()
        {
            if (AppOption.IsGroup)
                return Array.Empty<string>();

            var comparisonKey = AppOption.IsPathFromArgs
                ? AppOption.ComparisonKey
                : Message.ReadString(GetMessage(MessageKey.PleaseEnterTheVerificationKeyValueOrTheFilePathWhereTheKeyValueIsStored), true);

            var comparisonKeys = new List<string>();
            if (File.Exists(comparisonKey))
            {
                using (StreamReader reader = new StreamReader(comparisonKey))
                {
                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        comparisonKeys.Add(line);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(comparisonKey))
            {
                comparisonKeys.Add(comparisonKey);
            }

            return comparisonKeys.ToArray();
        }

    }
}
