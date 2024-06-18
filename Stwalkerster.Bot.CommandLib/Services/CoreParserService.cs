namespace Stwalkerster.Bot.CommandLib.Services;

using System;
using System.Text.RegularExpressions;
using Stwalkerster.Bot.CommandLib.Model;
using Stwalkerster.Bot.CommandLib.Services.Interfaces;

public class CoreParserService : ICoreParserService
{
    public CommandMessage ParseCommandMessage(string message, string nickname, string commandTrigger, bool isDirect, string initCharacter)
    {
        var directSkip = "";
        var overrideSilence = false;
        if (isDirect)
        {
            directSkip = "?";
            overrideSilence = true;
        }

        var initialMatch = "^";
        if (!string.IsNullOrWhiteSpace(initCharacter) && !isDirect)
        {
            initialMatch = $"(?:^|{Regex.Escape(initCharacter)})";
        }
        
        var regex =
            $"{initialMatch}(?:{commandTrigger}|(?:(?<botname>{nickname.ToLower()}|{nickname})(?:[:,] ?| ))){directSkip}(?<cmd>[\\S]+)(?: (?<args>.*?))?$";
        
        var validCommand = new Regex(regex);

        var m = validCommand.Match(message);

        if (m.Length <= 0)
        {
            return null;
        }

        overrideSilence |= m.Groups["botname"].Length > 0;

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

            if (string.Equals(commandName, nickname, StringComparison.CurrentCultureIgnoreCase))
            {
                overrideSilence = true;
                if (argList.Contains(" "))
                {
                    var strings = argList.Split(new []{' '}, 2);
                    commandName = strings[0];
                    argList = strings[1];
                }
                else
                {
                    commandName = argList;
                    argList = string.Empty;
                }
            }
        }

        var commandMessage = new CommandMessage(commandName, argList, overrideSilence);
        return commandMessage;
    }
}