using System.Reflection;
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

        private void displayFileInfoDetails(FileKeyInfo fileKeyInfo)
        {

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

        }

        private bool compareChecksums(FileKeyInfo fileKeyInfo)
        {

            if (string.IsNullOrEmpty(AppOption.ComparisonKey) == false)
            {
                var comparisonKeyLines = new List<string>();
                var isComparisonKeyFile = false;
                if (File.Exists(AppOption.ComparisonKey))
                {
                    isComparisonKeyFile = true;
                    using (StreamReader reader = new StreamReader(AppOption.ComparisonKey))
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
                    comparisonKeyLines.Add(AppOption.ComparisonKey);
                }

                var crc = fileKeyInfo.Crc32Normalized;
                var md5 = fileKeyInfo.Md5Normalized;
                var sha256 = fileKeyInfo.Sha256Normalized;

                var matchedHash = "";
                var matchedHashType = "";
                var matchedRowIndex = 0;
                var matchedColumnIndex = 0;

                foreach (var lineKey in comparisonKeyLines)
                {
                    matchedRowIndex++;
                    matchedColumnIndex = 0;

                    if (AppOption.OutCrcOption && (isComparisonKeyFile && lineKey.Contains(crc) || lineKey == crc))
                    {
                        matchedHash = crc;
                        matchedHashType = "CRC";
                        matchedColumnIndex = lineKey.IndexOf(crc) + 1;
                    }
                    else if (AppOption.OutMd5Option && (isComparisonKeyFile && lineKey.Contains(md5) || lineKey == md5))
                    {
                        matchedHash = md5;
                        matchedHashType = "MD5";
                        matchedColumnIndex = lineKey.IndexOf(md5) + 1;
                    }
                    else if (AppOption.OutSha256Option && (isComparisonKeyFile && lineKey.Contains(sha256) || lineKey == sha256))
                    {
                        matchedHash = sha256;
                        matchedHashType = "sha256";
                        matchedColumnIndex = lineKey.IndexOf(sha256) + 1;
                    }

                    if (matchedColumnIndex > 0)
                    {
                        if (isComparisonKeyFile)
                        {
                            Message.WriteLine($"{GetMessage(MessageKey.Matched, matchedHashType, matchedHash)}-{matchedRowIndex}.{matchedColumnIndex}", color: ConsoleColor.Green);
                        }
                        else
                        {
                            Message.WriteLine(GetMessage(MessageKey.Matched, matchedHashType, matchedHash), color: ConsoleColor.Green);
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

                Message.WarningLine(GetMessage(MessageKey.Miss, outKey), false);

            }

            return false;

        }

        private List<string>? getPaths()
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
                resultFilePaths = Directory.GetFiles(AppOption.FileOrDirectoryPath).ToList();
            }
            else if (File.Exists(AppOption.FileOrDirectoryPath))
            {
                resultFilePaths.Add(AppOption.FileOrDirectoryPath);
            }
            else
            {
                throw new Exception(GetMessage(MessageKey.TheInputFilePathDoesNotExist, AppOption.FileOrDirectoryPath));
            }

            return resultFilePaths;
        }

        private void getComparisonKey()
        {

            if (!AppOption.IsPathFromArgs)
                AppOption.ComparisonKey = Message.ReadString(GetMessage(MessageKey.PleaseEnterTheVerificationKeyValueOrTheFilePathWhereTheKeyValueIsStored), true);

        }

    }
}
