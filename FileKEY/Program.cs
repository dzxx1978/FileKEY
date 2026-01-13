using FileKEY;

try
{
    var options = args;
    var menuConfig = new MenuConfig();

    do
    {
        using var desktop = menuConfig.ShowMenu(options);
        await desktop.GanHuoer();
        options = Array.Empty<string>();
    } while (!AppStatus.IsHideMenu);
}
catch (Exception ex)
{
    Message.WarningLine(ex.Message, false);
    Message.Write(Language.GetHelpShown());
    return;
}


