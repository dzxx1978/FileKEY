using System.Runtime.InteropServices;
using System.Text;

namespace FileKEY;

public static class Message
{
    private static StringBuilder saveString = new();

    private static Dictionary<string,string> language = new();
    private static void doPrint(bool enter, string message = "")
    {

        message = translation(message);

        if (enter)
        {
            Console.WriteLine(message);
        }
        else
        {
            Console.Write(message);
        }

    }

    private static string translation(string message)
    {
        foreach (var item in language) { 
        
            message = message.Replace(item.Key, item.Value);
        
        }

        return message;
    }

    public static void LoadLanguage()
    {
        if (File.Exists("language.txt") == false) return;

        using (StreamReader reader = new StreamReader("language.txt"))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                var item = line.Split('=');
                if (item.Length == 2)
                {
                    if (item[0].Trim() != item[1].Trim())
                        language.Add(item[0].Trim(), item[1].Trim());
                }
            }
        }
    }

    /// <summary>
    /// 清屏（并显示已存储文本）
    /// </summary>
    /// <param name="delSave">是否清除已存储文本（否）</param>
    public static void Clear(bool delSave = false)
    {
        Console.Clear();
        if (delSave)
        {
            saveString.Clear();
        }
        else if (saveString.Length > 0)
        {
            doPrint(true, saveString.ToString());
        }
    }

    /// <summary>
    /// 存储新的文本
    /// </summary>
    /// <param name="message"></param>
    public static void AddSaveString(string message) {
        saveString.AppendLine(message.Trim());
    }

    /// <summary>
    /// 获取屏幕当前光标位置
    /// </summary>
    /// <param name="left">x坐标位置</param>
    /// <param name="top">y坐标位置</param>
    public static void GetPos(out int left, out int top)
    {
        try
        {
            var pos = Console.GetCursorPosition();
            left = pos.Left;
            top = pos.Top;
        }
        catch
        {
            left = 0;
            top = 0;
        }

    }

    /// <summary>
    /// 设置屏幕光标位置
    /// </summary>
    /// <param name="left">x坐标</param>
    /// <param name="top">y坐标</param>
    public static void SetPos(int left, int top) {
        try
        {
            Console.SetCursorPosition(left, top);
        }
        catch { }
    }

    /// <summary>
    /// 循环显示等待字符
    /// </summary>
    /// <param name="message"></param>
    /// <param name="delay"></param>
    /// <param name="left"></param>
    /// <param name="top"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task WriteLoop(string message, int delay = 500, int left = -1, int top = -1, CancellationToken cancellationToken = default)
    {

        if (left < 0 || top < 0)
        {
            GetPos(out left, out top);
        }

        var i = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (!Console.IsOutputRedirected)
                    Write(message.Substring(i, 1), left, top);

                i++;
                if (i >= message.Length) i = 0;

                await Task.Delay(delay, cancellationToken);
            }
            catch { }
        }

        Write(message.Substring(0, 1), left, top);
    }

    /// <summary>
    /// 显示文本行
    /// </summary>
    /// <param name="message">提示文本</param>
    /// <param name="save">是否储存文本（否）</param>
    /// <param name="color">颜色</param>
    public static void WriteLine(string message, bool save = false, ConsoleColor color = ConsoleColor.Gray)
    {
        if (Console.ForegroundColor == color)
        {
            doPrint(true, message);
        }
        else
        {
            Console.ForegroundColor = color;
            doPrint(true, message);
            Console.ResetColor();
        }

        if (save)
        {
            AddSaveString(message);
        }
    }

    /// <summary>
    /// 显示文本
    /// </summary>
    /// <param name="message">提示文本</param>
    /// <param name="color">颜色</param>
    public static void Write(string message, ConsoleColor color = ConsoleColor.Gray)
    {
        if (Console.ForegroundColor == color)
        {
            doPrint(false, message);
        }
        else
        {
            Console.ForegroundColor = color;
            doPrint(false, message);
            Console.ResetColor();
        }
    }

    /// <summary>
    /// 指定位置显示文本
    /// </summary>
    /// <param name="message">提示文本内容</param>
    /// <param name="left">x坐标</param>
    /// <param name="top">y坐标</param>
    /// <param name="color">颜色</param>
    public static void Write(string message, int left, int top, ConsoleColor color = ConsoleColor.Gray)
    {
        SetPos(left, top);
        Write(message, color);
    }

    /// <summary>
    /// 警告行文本（红字）
    /// </summary>
    /// <param name="message">提示文本</param>
    /// <param name="isWait">是否等待（是）</param>
    public static void WarningLine(string message, bool isWait = true)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        doPrint(true, message);
        Console.ResetColor();

        if (isWait)
        {
            Wait();
        }
    }

    /// <summary>
    /// 警告文本（红字）
    /// </summary>
    /// <param name="message">提示文本</param>
    /// <param name="isWait">是否等待（否）</param>
    public static void Warning(string message, bool isWait = false)
    {
        Write(message, ConsoleColor.Red);

        if (isWait)
        {
            Wait();
        }
    }

    /// <summary>
    /// 注意文本（黄底红字）
    /// </summary>
    /// <param name="message">文本内容</param>
    public static void Attention(string message)
    {
        Console.BackgroundColor = ConsoleColor.Yellow;
        Console.ForegroundColor = ConsoleColor.Red;
        doPrint(false, message);
        Console.ResetColor();
    }

    /// <summary>
    /// 注意文本（指定位置黄底红字）
    /// </summary>
    /// <param name="message">文本内容</param>
    /// <param name="left">指定列位置</param>
    /// <param name="top">指定行位置</param>
    public static void Attention(string message, int left, int top)
    {
        Console.BackgroundColor = ConsoleColor.Yellow;
        Write(message, left, top, ConsoleColor.Red);
        Console.ResetColor();

    }

    /// <summary>
    /// 等待按键输入（忽略调用前的按键缓存）
    /// </summary>
    /// <param name="message">提示文本内容</param>
    /// <param name="key">等待的按键（NoName代表任何按键）</param>
    /// <returns>按键信息</returns>
    public static ConsoleKeyInfo Wait(string? message = null, ConsoleKey key = ConsoleKey.NoName)
    {
        if (message is not null)
        {
            doPrint(true, message);
        }

        var t = DateTime.Now.Ticks / 100000;
        while (true)
        {
            var inkey = Console.ReadKey(true);
            var t1 = DateTime.Now.Ticks / 100000;

            if (t1 > t && (key == ConsoleKey.NoName || key == inkey.Key))
            {
                return inkey;
            }
        }

    }

    /// <summary>
    /// 设置菜单选项
    /// </summary>
    /// <param name="selected">选中的菜单行数</param>
    /// <param name="itemNum">显示的菜单行数</param>
    /// <param name="itemString">显示的菜单行文本</param>
    /// <param name="top">菜单起始位置</param>
    private static void ShowOptionString(int selected, int itemNum, string itemString, int top)
    {
        if (selected == itemNum)
        {
            Write($"->{itemNum}:{itemString}", 1, top + itemNum, ConsoleColor.Red);
        }
        else
        {
            Write($"  {itemNum}:{itemString}", 1, top + itemNum, ConsoleColor.White);
        }
    }

    /// <summary>
    /// 显示选择菜单（等待选择）
    /// </summary>
    /// <param name="defaultNum">默认选中菜单项</param>
    /// <param name="menus">菜单内容</param>
    /// <returns>被选中的菜单项</returns>
    public static int ShowSelectMenu(int defaultNum, string[] menus)
    {
        int select = defaultNum;
        int maxNum = menus.Length - 1;

        if (select > maxNum) select = maxNum;
        if (select < 1) select = 1;
        defaultNum = select;

        Clear();
        GetPos(out _, out var top);
        WriteLine(menus[0]);

        for (var i = 1; i < menus.Length; i++)
        {
            ShowOptionString(select, i, menus[i], top);
        }

        WriteLine("");
        GetPos(out var left, out var top1);
        Write($"请用键盘选择菜单，并按回车键确认！{select}");

        while (true)
        {
            var key = Wait();
            if (key.Key == ConsoleKey.Enter)
            {
                break;
            }
            else if (key.Key == ConsoleKey.DownArrow)
            {
                if (select < maxNum) select++;
            }
            else if (key.Key == ConsoleKey.UpArrow)
            {
                if (select > 1) select--;
            }
            else if (int.TryParse(key.KeyChar.ToString(), out int keyNum))
            {
                if (keyNum <= maxNum && keyNum >= 1)
                {
                    select = keyNum;
                }
            }

            if (defaultNum != select)
            {
                ShowOptionString(select, defaultNum, menus[defaultNum], top);
                ShowOptionString(select, select, menus[select], top);
                defaultNum = select;
                SetPos(left, top1);
                Write($"请用键盘选择菜单，并按回车键确认！{select}");
            }
        }

        Clear();
        WriteLine(menus[defaultNum]);

        return select;
    }

    /// <summary>
    /// 显示日期
    /// </summary>
    /// <param name="title">日期类型</param>
    /// <param name="riqi">日期字符串</param>
    /// <param name="outRiQi">转换后日期</param>
    /// <returns>字符串是否是日期</returns>
    public static bool ShowRiQi(string title, string riqi, out DateTime outRiQi)
    {
        var sf = DateTime.TryParse(riqi, out outRiQi);
        if (sf == false)
        {
            WarningLine($"{title}不正确");
        }
        else
        {
            WriteLine($"{title}：{outRiQi.ToString("yyyy年MM月dd日")}。", true);
            Clear();
        }
        return sf;
    }

    /// <summary>
    /// 获取字符串输入
    /// </summary>
    /// <param name="title">字符串名称</param>
    /// <returns>输入字符串</returns>
    public static string ReadString(string title)
    {
        doPrint(false, $"请输入{title}：");
        var UserName = Console.ReadLine() ?? "";
        return UserName;
    }


    /// <summary>
    /// 将整行文字抹去
    /// </summary>
    /// <param name="top">开始行</param>
    /// <param name="line">行数</param>
    /// <param name="left">行开始位置</param>
    public static void RemoveLines(int top, int line = 1, int left = 0)
    {
        try
        {
            if (line < 1) line = Console.BufferHeight - top;
            if (line < 1) line = 1;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var w = (Console.BufferWidth - left) / 2;
                if (w > 0)
                {
                    Console.MoveBufferArea(left + w, top, w, line, left, top);
                    Console.MoveBufferArea(left + w, top, w, line, left, top);
                }
            }
            else
            {
                GetPos(out int t1, out int l1);

                for (var t = 0; t < line; t++)
                {
                    SetPos(left, top + t);
                    for (var l = left; l < Console.WindowWidth; l++)
                    {
                        Write(" ");

                    }
                }

                SetPos(t1, l1);
            }
        }
        catch { }
    }

    /// <summary>
    /// 获取密码输入
    /// </summary>
    /// <param name="showChar">输入密码时显示的字符</param>
    /// <returns></returns>
    public static string ReadPassword(char showChar = '*')
    {
        doPrint(false, "请输入密码：");
        StringBuilder pwd = new StringBuilder();
        GetPos(out int left, out int top);
        while (true)
        {

            var keyInfo = Wait();

            // 如果用户按下Enter键，则退出循环
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                break;
            }

            // 如果用户按下Backspace键，则清空已输入字符
            if (keyInfo.Key == ConsoleKey.Backspace)
            {
                Write(new string(' ', pwd.Length), left, top);
                pwd.Clear();
            }
            else
            {
                // 将密码字符添加到StringBuilder中
                pwd.Append(keyInfo.KeyChar);
            }

            Write(new string(showChar, pwd.Length), left, top);

        }

        doPrint(true);

        return pwd.ToString();

    }

    /// <summary>
    /// 获取文件路径输入
    /// </summary>
    /// <param name="title">文件名称</param>
    /// <param name="defaultPath">默认路径</param>
    /// <returns>文件路径</returns>
    public static string ReadPath(string title, string defaultPath)
    {

        while (true)
        {
            if (string.IsNullOrEmpty(defaultPath) == false)
            {
                title += $"，使用默认输入({defaultPath})请直接回车：";
            }
            else
            {
                title += "：";
            }
            title = title.Replace("：，", "，").Replace(":，", "，").Replace("：：", "：").Replace(":：", "：").Trim();

            doPrint(true, title);

            var importString = Console.ReadLine();

            if (string.IsNullOrEmpty(importString))
            {
                if (string.IsNullOrEmpty(defaultPath) == false)
                {
                    importString = defaultPath;
                    GetPos(out int left, out int top);
                    Message.Write(importString, 0, top - 1);
                    SetPos(left, top);

                }
                else
                {
                    var menuNum = ShowSelectMenu(1, ["没有接收到任何文件夹", "重试", "退出"]);
                    if (menuNum == 1)
                    {
                        continue;
                    }
                }
            }
            else
            {
                defaultPath = importString;
            }

            break;
        }
        return defaultPath.Replace("\"","").Trim();
    }

    /// <summary>
    /// 获取数字输入
    /// </summary>
    /// <param name="title">数字名称</param>
    /// <param name="returnLength">数字允许最大长度（0=不限制）</param>
    /// <param name="defaultValue">默认数字</param>
    /// <returns>数字</returns>
    public static string ReadNumber(string title, int returnLength = 0, string defaultValue = "")
    {

        string numString = "";

        while (true)
        {
            doPrint(false, $"请输入{title}：");
            numString = Console.ReadLine() ?? "";
            if (string.IsNullOrEmpty(numString) && string.IsNullOrEmpty(defaultValue) == false)
            {
                numString = defaultValue;
            }
            if (int.TryParse(numString, out int xkhNum))
            {
                if (returnLength > 0)
                {
                    numString = xkhNum.ToString(new string('0', returnLength));
                }
                else
                {
                    numString = xkhNum.ToString();
                }
            }
            else
            {
                doPrint(true, $"{title}应该是{returnLength}位数字！请重新输入。");
                continue;
            }
            break;
        }

        return numString;
    }

    /// <summary>
    /// 获取数字键入
    /// </summary>
    /// <param name="defaultValue">默认值（并限制输入最大长度）</param>
    /// <param name="left">输入x起始位置</param>
    /// <param name="top">输入y起始位置</param>
    /// <param name="next">光标在（1y2m3d）的位置</param>
    /// <param name="isPositive">是否只能是正数（是）</param>
    /// <param name="isDecimal">是否只能是小数（否）</param>
    /// <returns></returns>
    public static int ReadNumber(string defaultValue, int left, int top, ref int next, bool isPositive = true, bool isDecimal = false)
    {
        int x = 0;
        var keys = new StringBuilder(defaultValue);
        var returnkeys = 0;

        while (true)
        {

            Attention(keys.ToString(), left, top);
            SetPos(left + x, top);
            var keyInfo = Wait();

            // 如果用户按下Enter键，则退出循环
            if (keyInfo.Key == ConsoleKey.Enter || keyInfo.Key == ConsoleKey.UpArrow || keyInfo.Key == ConsoleKey.DownArrow)
            {
                var sf = int.TryParse(keys.ToString().Replace(" ", ""), out returnkeys);
                if (sf == false)
                {
                    sf = int.TryParse(defaultValue, out returnkeys);
                    if (sf == false)
                    {
                        returnkeys = 0;
                    }
                }
                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    next--;
                }
                else
                {
                    next++;
                }
                break;
            }
            else if (keyInfo.Key == ConsoleKey.RightArrow)
            {
                // 右键
                if (x < keys.Length - 1) x++;
            }
            else if (keyInfo.Key == ConsoleKey.LeftArrow)
            {
                // 左键
                if (x > 0) x--;
            }
            else if (keyInfo.Key == ConsoleKey.Backspace)
            {
                // 退格键
                keys.Remove(x, 1);
                keys.Insert(x, ' ');
                if (x > 0) x--;
            }
            else if (keyInfo.Key == ConsoleKey.Delete)
            {
                // del键
                keys.Remove(x, 1);
                keys.Insert(x, ' ');
            }
            else if ("0123456789".Contains(keyInfo.KeyChar) || keyInfo.KeyChar == '-' && isPositive == false || keyInfo.KeyChar == '.' && isDecimal == true)
            {
                // 数字
                if (x == 0)
                {
                    if (keyInfo.KeyChar != '.' && (keyInfo.KeyChar != '-' || keys.ToString().Contains("-") == false))
                    {
                        keys.Clear();
                        keys.Append(keyInfo.KeyChar + (new string(' ', defaultValue.Length - 1)));
                    }
                }
                else
                {
                    if (keyInfo.KeyChar != '-' && (keyInfo.KeyChar != '.' || keys.ToString().Contains(".") == false))
                    {
                        keys.Remove(x, 1);
                        keys.Insert(x, keyInfo.KeyChar);
                    }
                }

                if (x < keys.Length - 1) x++;
            }

        }

        return returnkeys;
    }

    /// <summary>
    /// 获取日期（年）输入
    /// </summary>
    /// <param name="defaultValue">默认日期</param>
    /// <param name="next">光标在（1y2m3d）的位置</param>
    /// <returns>年数</returns>
    public static int ReadDateYear(DateTime defaultValue, ref int next)
    {
        Write("请输入年份:");
        GetPos(out var left, out var top);
        Attention(defaultValue.Year.ToString());
        Write(defaultValue.ToString("年MM月dd日"));

        var next1 = next;
        var year = ReadNumber(defaultValue.ToString("yyyy"), left, top, ref next);
        if (year < 1900 || year > 9999)
        {
            year = defaultValue.Year;
            next = next1;
        }
        return year;
    }

    /// <summary>
    /// 获取日期（月）输入
    /// </summary>
    /// <param name="defaultValue">默认日期</param>
    /// <param name="next">光标在（1y2m3d）的位置</param>
    /// <returns>月数</returns>
    public static int ReadDateMonth(DateTime defaultValue, ref int next)
    {
        Write("请输入月份:");
        Write(defaultValue.ToString("yyyy年"));
        GetPos(out var left, out var top);
        Attention(defaultValue.ToString("MM"));
        Write(defaultValue.ToString("月dd日"));

        var next1 = next;
        var month = ReadNumber(defaultValue.ToString("MM"), left, top, ref next);

        if (month < 1 || month > 12)
        {
            month = defaultValue.Month;
            next = next1;
        }
        return month;
    }

    /// <summary>
    /// 获取日期（日）输入
    /// </summary>
    /// <param name="defaultValue">默认日期</param>
    /// <param name="next">光标在（1y2m3d）的位置</param>
    /// <returns>日数</returns>
    public static int ReadDateDay(DateTime defaultValue, ref int next)
    {
        Write("请输入哪天:");
        Write(defaultValue.ToString("yyyy年MM月"));
        GetPos(out var left, out var top);
        Attention(defaultValue.ToString("dd"));
        Write("日");

        var next1 = next;
        var day = ReadNumber(defaultValue.ToString("dd"), left, top, ref next);
        if (day < 1 || day > 31)
        {
            day = defaultValue.Day;
            next = next1;
        }
        return day;
    }

    /// <summary>
    /// 获取日期输入
    /// </summary>
    /// <param name="title">日期名称</param>
    /// <param name="defaultDate">默认日期(null=Now)</param>
    /// <returns>yyyy-MM-dd</returns>
    public static DateTime ReadDate(string title, DateTime? defaultDate = null)
    {
        if (defaultDate is null) defaultDate = DateTime.Now;

        var year = defaultDate?.Year;
        var month = defaultDate?.Month;
        var day = defaultDate?.Day;

        var sf = DateTime.TryParse($"{year}-{month}-{day}", out var outRq);
        var next = 1;

        Clear();
        WriteLine(title);
        GetPos(out var left, out var top);

        while (next < 4)
        {
            if (next < 1) next = 1;
            switch (next)
            {
                case 1:
                    SetPos(left, top);
                    year = ReadDateYear(outRq, ref next);
                    break;
                case 2:
                    SetPos(left, top);
                    month = ReadDateMonth(outRq, ref next);
                    break;
                case 3:
                    SetPos(left, top);
                    day = ReadDateDay(outRq, ref next);
                    break;
            }

            sf = DateTime.TryParse($"{year}-{month}-{day}", out outRq);
            while (sf == false && day > 25)
            {
                day--;
                sf = DateTime.TryParse($"{year}-{month}-{day}", out outRq);
            }
        }

        return outRq;
    }


}
