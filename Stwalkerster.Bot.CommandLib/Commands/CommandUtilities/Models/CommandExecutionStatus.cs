namespace Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models
{
    public class CommandExecutionStatus
    {
        public CommandExecutionStatus()
        {
            this.AclStatus = CommandAclStatus.Prerun;
        }

        public CommandAclStatus AclStatus { get; internal set; }
        public string MainFlags { get; internal set; }
        public string SubcommandFlags { get; internal set; }
    }
}