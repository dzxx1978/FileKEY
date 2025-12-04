using FileKEY;

var path = "";
var inKey = "";

var outType = 1;
var outCRC = 1;
var outMD5 = 1;
var outSha256 = 1;

var outShow = true;

var ifArgsPath = false;

Message.LoadLanguage();

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
                Message.WriteLine("FileKEY 1.0 by zxx 2025");
                Message.WriteLine("FileKEY [path] [key] [-0tcms]");
                Message.WriteLine(" -0 small print");
                Message.WriteLine(" -t only type");
                Message.WriteLine(" -c only crc");
                Message.WriteLine(" -m only md5");
                Message.WriteLine(" -s only sha256");
                return;
            }
            if (parameter.Contains("T"))
            {
                parameter = parameter.Replace("T", "");
                outType = 2;
            }
            if (parameter.Contains("C"))
            {
                parameter = parameter.Replace("C", "");
                outCRC = 2;
            }
            if (parameter.Contains("M"))
            {
                parameter = parameter.Replace("M", "");
                outMD5 = 2;
            }
            if (parameter.Contains("S"))
            {
                parameter = parameter.Replace("S", "");
                outSha256 = 2;
            }
            if (parameter.Contains("0"))
            {
                parameter = parameter.Replace("0", "");
                outShow = false;
            }

            if (!string.IsNullOrEmpty(parameter))
            {
                Message.WriteLine($"无法识别的参数：-{parameter}");
                Message.WriteLine("请使用（-v）选项显示更多信息");
                return;
            }
        }
        else if (string.IsNullOrEmpty(path))
        {
            ifArgsPath = true;
            path = arr[i];
        }
        else if (string.IsNullOrEmpty(inKey))
        {
            inKey = arr[i];
        }
        else
        {
            Message.WriteLine($"参数过多：{arr[i]}");
            Message.WriteLine("请使用（-v）选项显示更多信息");
            return;
        }
    }

    if (outType == 2 || outCRC == 2 || outMD5 == 2 || outSha256 == 2)
    {
        outType--;
        outCRC--;
        outMD5--;
        outSha256--;
    }
}

if (outShow)
{
    if (!string.IsNullOrEmpty(path))
        Message.WriteLine($"argsfile:{path}");
    if (!string.IsNullOrEmpty(inKey))
        Message.WriteLine($"argskey:{inKey}");
}

while (true)
{
    if (!ifArgsPath)
        path = Message.ReadPath("请输入文件路径", path);

    if (string.IsNullOrEmpty(path) || path.ToLower() == "exit")
    {
        break;
    }

    var filePaths = new List<string>();
    if (Directory.Exists(path))
    {
        filePaths = Directory.GetFiles(path).ToList();
    }
    else if (File.Exists(path))
    {
        filePaths.Add(path);
    }
    else
    {
        Message.WarningLine($"输入的文件路径{path}不存在", false);
        path = "";
        continue;
    }

    int left = 0;
    int top = 0;

    var fileKey = new FileKey();
    foreach (var file in filePaths)
    {
        Task task1 = Task.Run(() => { });
        var tk = new CancellationTokenSource();
        if (!ifArgsPath)
        {
            Message.Write("稍等 >>");
            Message.GetPos(out left, out top);
            task1 = Message.WriteLoop("*>", cancellationToken: tk.Token);
        }

        var key = await fileKey.GetFileKeyInfo(file, $"{outType}{outCRC}{outMD5}{outSha256}", tk.Token);

        if (!ifArgsPath)
        {
            tk.Cancel();
            await task1;
            Message.RemoveLines(top);
            Message.SetPos(0, top + 1);
        }

        if (key.Exists == true)
        {
            var crc = key.CRC.ToString("X8");
            var md5 = key.MD5.Replace("-", "").ToLower();
            var sha256 = key.Sha256.Replace("-", "").ToLower();

            if (outShow)
            {
                Message.WriteLine($"name:{key.Name}");
                Message.WriteLine($"time:{key.Time.ToString("yyyy-MM-dd HH:mm:ss")}");
                Message.WriteLine($"size:{key.Size}({key.Length}B)");
                if (outType == 1)
                    Message.WriteLine($"type:{key.Type}");
                if (outCRC == 1)
                    Message.WriteLine($"crc:{crc}");
                if (outMD5 == 1)
                    Message.WriteLine($"md5:{md5}");
                if (outSha256 == 1)
                    Message.WriteLine($"sha256:{sha256}");
            }
            if (string.IsNullOrEmpty(inKey) == false)
            {
                if (crc == inKey)
                {
                    Message.WriteLine($"same-CRC", color: ConsoleColor.Green);
                }
                else if (md5 == inKey)
                {
                    Message.WriteLine($"same-MD5", color: ConsoleColor.Green);
                }
                else if (sha256 == inKey)
                {
                    Message.WriteLine($"same-sha256", color: ConsoleColor.Green);
                }
                else
                {
                    var outKey = crc.Length == inKey.Length ? crc : md5.Length == inKey.Length ? md5 : sha256.Length == inKey.Length ? sha256 : $"NoKeyTheLengthIs({inKey.Length})";
                    Message.WarningLine($"different-{outKey}", false);
                }
            }
        }
        else
        {
            Message.WarningLine($"err:{key.ErrMessage}", false);
        }

    }

    if (!ifArgsPath && !Console.IsOutputRedirected)
    {
        Message.Attention("显示完毕，按回车继续下一个。");
        Message.Wait("", ConsoleKey.Enter);
    }
    else
    {
        if (outShow)
            Message.WriteLine("END");
        break;
    }
}