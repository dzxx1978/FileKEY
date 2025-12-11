using System.Threading.Tasks;
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

                var comparisonKeys = await getComparisonKeys();

                foreach (var file in filePaths)
                {
                    var fileKeyInfo = await readFileInfo(file);

                    compareChecksums(fileKeyInfo, comparisonKeys);
                    if (displayFileInfo(fileKeyInfo, filePaths, comparisonKeys))
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

        private bool displayFileInfo(FileKeyInfo fileKeyInfo, string[] filePaths, string[] comparisonKeys)
        {
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

            if (fileKeyInfo.ComparisonInfo.IsEqual)
            {
                Message.WriteLine(fileKeyInfo.ComparisonInfo.Message, color: ConsoleColor.Green);
                if (filePaths.Length > 1 && comparisonKeys.Length == 1)
                {
                    if (!AppOption.IsDetailedInfoShown)
                    {
                        Message.WriteLine(Path.GetFullPath(fileKeyInfo.Path));
                    }
                    return true;
                }
            }
            else if (filePaths.Length == 1 || comparisonKeys.Length > 1)
            {
                Message.WarningLine(fileKeyInfo.ComparisonInfo.Message, false);
            }

            return false;
        }

        private bool compareChecksums(FileKeyInfo fileKeyInfo, string[] comparisonKeys)
        {
            if (!fileKeyInfo.Exists || comparisonKeys.Length == 0) return false;

            var crc = fileKeyInfo.Crc32Normalized;
            var md5 = fileKeyInfo.Md5Normalized;
            var sha256 = fileKeyInfo.Sha256Normalized;

            var matchedHash = "";
            var matchedHashType = "";
            var matchedRowIndex = 0;
            var matchedColumnIndex = 0;

            var isComparisonKeyFile = comparisonKeys.Length > 1;
            foreach (var lineKey in comparisonKeys)
            {
                matchedRowIndex++;
                matchedColumnIndex = 0;

                if (AppOption.OutCrcOption && (isComparisonKeyFile && lineKey.Contains(crc) || lineKey == crc))
                {
                    matchedHash = crc;
                    matchedHashType = "CRC";
                    matchedColumnIndex = lineKey.IndexOf(crc) + 1;
                    fileKeyInfo.ComparisonInfo.IsCrc32Equal = true;
                }
                else if (AppOption.OutMd5Option && (isComparisonKeyFile && lineKey.Contains(md5) || lineKey == md5))
                {
                    matchedHash = md5;
                    matchedHashType = "MD5";
                    matchedColumnIndex = lineKey.IndexOf(md5) + 1;
                    fileKeyInfo.ComparisonInfo.IsMd5Equal = true;
                }
                else if (AppOption.OutSha256Option && (isComparisonKeyFile && lineKey.Contains(sha256) || lineKey == sha256))
                {
                    matchedHash = sha256;
                    matchedHashType = "sha256";
                    matchedColumnIndex = lineKey.IndexOf(sha256) + 1;
                    fileKeyInfo.ComparisonInfo.IsSha256Equal = true;
                }

                if (matchedColumnIndex > 0)
                {
                    fileKeyInfo.ComparisonInfo.InKeyColumn = matchedColumnIndex;
                    fileKeyInfo.ComparisonInfo.InKeyRow = matchedRowIndex;

                    if (isComparisonKeyFile)
                    {
                        fileKeyInfo.ComparisonInfo.Message = GetMessage(MessageKey.MatchedInKeysFile, matchedHashType, matchedHash, matchedRowIndex, matchedColumnIndex);
                    }
                    else
                    {
                        fileKeyInfo.ComparisonInfo.Message = GetMessage(MessageKey.Matched, matchedHashType, matchedHash);
                    }
                    return true;
                }
            }

            var outKey = AppOption.OnlyOutHashOption(nameof(AppOption.OutCrcOption)) ? crc
                : AppOption.OnlyOutHashOption(nameof(AppOption.OutMd5Option)) ? md5
                : AppOption.OnlyOutHashOption(nameof(AppOption.OutSha256Option)) ? sha256
                : !isComparisonKeyFile && AppOption.OutCrcOption && crc.Length == AppOption.ComparisonKey.Length ? crc
                : !isComparisonKeyFile && AppOption.OutMd5Option && md5.Length == AppOption.ComparisonKey.Length ? md5
                : !isComparisonKeyFile && AppOption.OutSha256Option && sha256.Length == AppOption.ComparisonKey.Length ? sha256
                : isComparisonKeyFile ? GetMessage(MessageKey.NoKeyInFile, Path.GetFileName(AppOption.ComparisonKey))
                : GetMessage(MessageKey.NoKeyTheLengthIs, AppOption.ComparisonKey.Length);

            fileKeyInfo.ComparisonInfo.Message = GetMessage(MessageKey.Miss, outKey);

            return false;

        }

        private string[]? getPaths()
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
