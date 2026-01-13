using FileKEY;

try
{
    var options = args;
    do
    {
        new MenuConfig().ShowMenu(options);
        await new Desktop().GanHuoer();
        options = Array.Empty<string>();
    } while (!AppStatus.IsHideMenu);
}
catch (Exception ex)
{
    Message.WarningLine(ex.Message, false);
    Message.Write(Language.GetHelpShown());
    return;
}


