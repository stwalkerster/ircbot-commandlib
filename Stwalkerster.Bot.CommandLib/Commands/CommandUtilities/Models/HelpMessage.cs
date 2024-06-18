namespace Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    
public class HelpMessage
{        
    public HelpMessage(string commandName, string syntax, string text)
        : this(commandName, new List<string> {syntax}, new List<string> {text})
    {
    }
        
    public HelpMessage(string commandName, IEnumerable<string> syntax, string text)
        : this(commandName, syntax, new List<string> {text})
    {
    }
        
    public HelpMessage(string commandName, string syntax, IEnumerable<string> text)
        : this(commandName, new List<string> {syntax}, text)
    {
    }
        
    public HelpMessage(string commandName, IEnumerable<string> syntax, IEnumerable<string> text)
    {
        this.CommandName = commandName;
        this.Syntax = syntax;
        this.Text = text;
    }

    public string CommandName { get; private set; }
    public IEnumerable<string> Syntax { get; }
    public IEnumerable<string> Text { get; }

    public IEnumerable<CommandResponse> ToCommandResponses(string commandTrigger, string commandName, string syntaxPrefix)
    {
        this.CommandName = this.CommandName ?? commandName;

        if (this.CommandName == null)
        {
            throw new ArgumentOutOfRangeException(nameof(commandName), "Command name must be specified");
        }

        if (!string.IsNullOrWhiteSpace(syntaxPrefix))
        {
            syntaxPrefix = " " + syntaxPrefix.Trim();
        }
            
        var messages = new List<CommandResponse>();

        messages.AddRange(
            this.Syntax.Select(
                syntax =>
                    new CommandResponse
                    {
                        Message =
                            string.Format("{2}{0}{3} {1}", this.CommandName, syntax, commandTrigger, syntaxPrefix), 
                        Destination = CommandResponseDestination.PrivateMessage,
                        Type = CommandResponseType.Notice
                    }));

        messages.AddRange(
            this.Text.Select(
                helpText =>
                    new CommandResponse
                    {
                        Message = $"   {helpText}", 
                        Destination = CommandResponseDestination.PrivateMessage,
                        Type = CommandResponseType.Notice
                    }));

        return messages;
    }
}