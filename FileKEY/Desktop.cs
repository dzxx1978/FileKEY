using static FileKEY.Language;

namespace FileKEY
{
    public class Desktop
    {
        private FileKey fileKey;

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
                string[]? filePaths;

                try
                {
                    filePaths = getFilePaths();

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

                var comparisonKeys = await getComparisonKeys();

                foreach (var file in filePaths)
                {
                    var fileKeyInfo = await readFileInfo(file);

                    var comparisonInfo = compareChecksums(fileKeyInfo, comparisonKeys);
                    if (displayFileInfo(comparisonInfo, filePaths.Length))
                    {
                        break;
                    }
                }


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

        private async Task<FileKeyInfo> readFileInfo(string file)
        {

            Task task1 = Task.Run(() => { });
            var tk = new CancellationTokenSource();

            if (AppOption.IsDetailedInfoShown)
            {
                Message.Write(GetMessage(MessageKey.Wait));
                task1 = Message.WriteLoop(">*", cancellationToken: tk.Token);
            }

            var key = await fileKey.GetFileKeyInfo(file, tk.Token);

            tk.Cancel();
            await task1;

            if (AppOption.IsDetailedInfoShown)
                Message.WriteLine(GetMessage(MessageKey.ProcessCompleted));

            return key;
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
                    Message.WriteLine($"type:{fileKeyInfo.TypeName}");
                if (AppOption.OutCrcOption)
                    Message.WriteLine($"crc:{fileKeyInfo.Crc32Normalized}");
                if (AppOption.OutMd5Option)
                    Message.WriteLine($"md5:{fileKeyInfo.Md5Normalized}");
                if (AppOption.OutSha256Option)
                    Message.WriteLine($"sha256:{fileKeyInfo.Sha256Normalized}");
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

            if (!AppOption.IsPathFromArgs)
                AppOption.FileOrDirectoryPath = Message.ReadPath(GetMessage(MessageKey.PleaseEnterTheFilePath), AppOption.FileOrDirectoryPath);

            if (string.IsNullOrEmpty(AppOption.FileOrDirectoryPath) || AppOption.FileOrDirectoryPath.ToLower() == "exit")
            {
                return null;
            }

            var resultFilePaths = new List<string>();
            if (Directory.Exists(AppOption.FileOrDirectoryPath))
            {
                return Directory.GetFiles(AppOption.FileOrDirectoryPath);
            }
            else if (File.Exists(AppOption.FileOrDirectoryPath))
            {
                return [AppOption.FileOrDirectoryPath];
            }
            else
            {
                throw new Exception(GetMessage(MessageKey.TheInputFilePathDoesNotExist, AppOption.FileOrDirectoryPath));
            }

        }

        private async Task<string[]> getComparisonKeys()
        {

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
