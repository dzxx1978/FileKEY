using FileKEY;

try
{
    AppOption.parseCommandLineArgs(args.ToArray());
}
catch (Exception ex)
{
    AppOption.IsHelpShownAndExit = true;
    AppOption.IsDetailedInfoShown = true;
    Message.WarningLine(ex.Message, false);
}

await new Desktop().GanHuoer();
