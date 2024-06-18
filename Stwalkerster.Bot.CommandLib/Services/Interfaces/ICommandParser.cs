namespace Stwalkerster.Bot.CommandLib.Services.Interfaces;

using System;
using System.Collections.Generic;
using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
using Stwalkerster.Bot.CommandLib.Commands.Interfaces;
using Stwalkerster.Bot.CommandLib.Model;
using Stwalkerster.IrcClient.Interfaces;
using Stwalkerster.IrcClient.Model.Interfaces;
    
public interface ICommandParser
{
    ICommand GetCommand(CommandMessage commandMessage, IUser user, string destination, IIrcClient client);

    void Release(ICommand command);

    CommandMessage ParseCommandMessage(string message, string nickname, bool isDirect);

    RedirectionResult ParseRedirection(IEnumerable<string> inputArguments);

    /// <summary>
    /// Registers a command
    /// </summary>
    /// <param name="commandName">
    /// The public name of the command, used to invoke this command from IRC
    /// </param>
    /// <param name="implementation">
    /// The type implementing this command
    /// </param>
    void RegisterCommand(string commandName, Type implementation);

    /// <summary>
    /// Registers a command to a specific channel
    /// </summary>
    /// <param name="commandName">
    /// The public name of the command, used to invoke this command from IRC
    /// </param>
    /// <param name="implementation">
    /// The type implementing this command
    /// </param>
    /// <param name="channel">
    /// The channel to limit this registration to
    /// </param>
    void RegisterCommand(string commandName, Type implementation, string channel);

    /// <summary>
    /// unegisters a command
    /// </summary>
    /// <param name="commandName">
    /// The public name of the command, used to invoke this command from IRC
    /// </param>
    void UnregisterCommand(string commandName);
        
    /// <summary>
    /// Unregisters a command to a specific channel
    /// </summary>
    /// <param name="commandName">
    /// The public name of the command, used to invoke this command from IRC
    /// </param>
    /// <param name="channel">
    /// The channel this registration was limited to
    /// </param>
    void UnregisterCommand(string commandName, string channel);

    Type GetRegisteredCommand(string commandName);
        
    Type GetRegisteredCommand(string commandName, string destination);
        
    Dictionary<string, Dictionary<CommandRegistration, Type>> GetCommandRegistrations();
}