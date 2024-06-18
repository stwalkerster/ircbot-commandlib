namespace Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;

public class CommandExecutionStatus
{
    public CommandAclStatus AclStatus { get; internal set; } = CommandAclStatus.Prerun;
    public string MainFlags { get; internal set; }
    public string SubcommandFlags { get; internal set; }
}