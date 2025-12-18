using System.Runtime.InteropServices;
using System.Text;

namespace FileKEY;

public static class Message
{
    private static StringBuilder saveString = new();

    private static void doPrint(bool enter, string message = "")
    {

        if (enter)
        {
            Console.WriteLine(message);
        }
        else
        {
            Console.Write(message);
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
    public static void AddSaveString(string message)
    {
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
    public static void SetPos(int left, int top)
    {
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
    /// <param name="consoleKeys">等待的按键（NoName代表任何按键）</param>
    /// <returns>按键信息</returns>
    public static ConsoleKeyInfo Wait(string? message = null, params ConsoleKey[] consoleKeys)
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

            if (t1 > t)
            {
                if (consoleKeys.Length == 0)
                {
                    return inkey;
                }
                foreach (var consoleKey in consoleKeys)
                {
                    if (consoleKey == ConsoleKey.NoName || consoleKey == inkey.Key)
                    {
                        return inkey;
                    }
                }
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
            Write($" ->{itemNum}:{itemString}", 0, top + itemNum, ConsoleColor.Red);
        }
        else
        {
            Write($"   {itemNum}:{itemString}", 0, top + itemNum, ConsoleColor.White);
        }
    }

    /// <summary>
    /// 显示选择菜单（等待选择）
    /// </summary>
    /// <param name="selected">默认选中菜单项</param>
    /// <param name="menus">菜单内容</param>
    /// <returns>被选中的菜单项</returns>
    public static int ShowSelectMenu(int selected, string[] menus)
    {
        int select = selected;
        int maxNum = menus.Length - 1;

        if (select > maxNum) select = maxNum;
        if (select < 1) select = 1;
        selected = select;

        Clear();
        GetPos(out _, out var top);
        WriteLine(menus[0]);

        for (var i = 1; i < menus.Length; i++)
        {
            ShowOptionString(select, i, menus[i], top);
        }

        WriteLine("");
        Write(Language.GetMessage(Language.MessageEnum.MenuSelected));
        GetPos(out var left, out var top1);
        Write(select.ToString());

        while (true)
        {
            var key = Wait();
            if (key.Key == ConsoleKey.Enter)
            {
                break;
            }
            switch (key.Key)
            {
                case ConsoleKey.DownArrow:
                    if (select < maxNum) select++;
                    break;
                case ConsoleKey.UpArrow:
                    if (select > 1) select--;
                    break;
                case ConsoleKey.PageDown:
                    select = maxNum;
                    break;
                case ConsoleKey.PageUp:
                    select = 1;
                    break;
                default:
                    if (int.TryParse(key.KeyChar.ToString(), out int keyNum))
                    {
                        if (keyNum <= maxNum && keyNum >= 1)
                        {
                            select = keyNum;
                        }
                    }
                    break;
            }

            if (selected != select)
            {
                ShowOptionString(select, selected, menus[selected], top);
                ShowOptionString(select, select, menus[select], top);
                selected = select;
                Write(select.ToString(), left, top1);
            }
        }

        Clear();
        WriteLine(menus[selected]);

        return select;
    }

    /// <summary>
    /// 获取字符串输入
    /// </summary>
    /// <param name="title">字符串名称</param>
    /// <returns>输入字符串</returns>
    public static string ReadString(string title, bool enter = false)
    {
        doPrint(enter, title);
        return Console.ReadLine() ?? "";
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
                title = Language.GetMessage(Language.MessageEnum.PleaseEnterTheFilePathUseDefaultInputPleaseEnterDirectly, defaultPath);
            }

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
            }
            else
            {
                defaultPath = importString;
            }

            break;
        }
        return defaultPath.Replace("\"", "").Trim();
    }

    /// <summary>
    /// 获取数字输入
    /// </summary>
    /// <param name="title">数字名称</param>
    /// <param name="returnLength">数字允许最大长度（0=不限制）</param>
    /// <param name="defaultValue">默认数字</param>
    /// <returns>数字</returns>
    public static string ReadNumber(string title, int returnLength = 0, decimal defaultValue = 0)
    {

        string numString = "";

        while (true)
        {
            doPrint(false, title);
            numString = Console.ReadLine() ?? "";
            if (string.IsNullOrEmpty(numString))
            {
                numString = defaultValue.ToString();
            }
            if (decimal.TryParse(numString, out decimal outNum))
            {
                if (returnLength > 0)
                {
                    numString = outNum.ToString(new string('0', returnLength));
                }
                else
                {
                    numString = outNum.ToString();
                }
            }
            else
            {
                numString = defaultValue.ToString();
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

}
