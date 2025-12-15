using FileKEY;

try
{
    AppOption.ShowMenu(args.ToList());
}
catch (Exception ex)
{
    Message.WarningLine(ex.Message, false);
    Message.Write(Language.GetHelpShown());
    return;
}

await new Desktop().GanHuoer();
