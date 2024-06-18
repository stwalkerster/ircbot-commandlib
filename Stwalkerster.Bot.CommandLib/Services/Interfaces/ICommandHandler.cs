namespace Stwalkerster.Bot.CommandLib.Services.Interfaces;

using System;
using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
using Stwalkerster.IrcClient.Events;

public interface ICommandHandler
{
    /// <summary>
    /// Called on new messages received by the IRC client
    /// </summary>
    void OnMessageReceived(object sender, MessageReceivedEventArgs e);

    event EventHandler<CommandExecutedEventArgs> CommandExecuted;
        
    ISilentModeConfiguration SilentModeConfiguration { get; set; }
}