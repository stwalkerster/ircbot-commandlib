namespace Stwalkerster.Bot.CommandLib.Services
{
    using System.Text.RegularExpressions;
    using Stwalkerster.Bot.CommandLib.Model;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;

    public class CoreParserService : ICoreParserService
    {
        public CommandMessage ParseCommandMessage(string message, string nickname, string commandTrigger)
        {
            var validCommand =
                new Regex(
                    @"^(?:" + commandTrigger + @"(?:(?<botname>(?:" + nickname + @")|(?:"
                    + nickname.ToLower() + @")) )?(?<cmd>[" + "0-9a-z-_" + "]+)|(?<botname>(?:" + nickname
                    + @")|(?:" + nickname.ToLower() + @"))[ ,>:](?: )?(?<cmd>[" + "0-9a-z-_"
                    + "]+))(?: )?(?<args>.*?)(?:\r)?$");

            Match m = validCommand.Match(message);

            if (m.Length > 0)
            {
                var overrideSilence = m.Groups["botname"].Length > 0;

                string commandName;
                if (m.Groups["cmd"].Length > 0)
                {
                    commandName = m.Groups["cmd"].Value.Trim();
                }
                else
                {
                    return null;
                }

                var argList = string.Empty;
                if (m.Groups["args"].Length > 0)
                {
                    argList = m.Groups["args"].Value.Trim();
                }

                var commandMessage = new CommandMessage(commandName, argList, overrideSilence);
                return commandMessage;
            }

            return null;
        }
    }
}