namespace Stwalkerster.Bot.CommandLib.Services;

using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Logging;
using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
using Stwalkerster.Bot.CommandLib.Services.Interfaces;
using Stwalkerster.IrcClient.Events;
using Stwalkerster.IrcClient.Messages;

public class CommandHandler : ICommandHandler
{
    private readonly ICommandParser commandParser;
    private readonly IConfigurationProvider configProvider;
    private readonly ILogger<CommandHandler> logger;
    public event EventHandler<CommandExecutedEventArgs> CommandExecuted;

    public CommandHandler(ICommandParser commandParser, IConfigurationProvider configProvider, ILogger<CommandHandler> logger)
    {
        this.commandParser = commandParser;
        this.configProvider = configProvider;
        this.logger = logger;
    }
        
    public ISilentModeConfiguration SilentModeConfiguration { get; set; }
        
    /// <summary>
    /// Event handler to be called on new messages received by the IRC client
    /// </summary>
    public void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        if (e.IsNotice)
        {
            return;
        }

        ThreadPool.QueueUserWorkItem(this.ProcessMessageAsync, e);
    }

    private void ProcessMessageAsync(object state)
    {
        var globalStopwatch = Stopwatch.StartNew();
        var eventArgs = (MessageReceivedEventArgs)state;
        var client = eventArgs.Client;
        var message = eventArgs.Message;

        var commandMessage = this.commandParser.ParseCommandMessage(
            message,
            client.Nickname,
            eventArgs.Target == client.Nickname);
            
        var command = this.commandParser.GetCommand(
            commandMessage,
            eventArgs.User,
            eventArgs.Target,
            client);

        if (command == null)
        {
            globalStopwatch.Stop();
            return;
        }
            
        if (!commandMessage.OverrideSilence 
            && this.SilentModeConfiguration != null
            && this.SilentModeConfiguration.BotIsSilent(eventArgs.Target, commandMessage))
        {
            this.logger.LogInformation("Skipping command; bot is in silent mode in {Target}", eventArgs.Target);
            globalStopwatch.Stop();
            return;
        }

        try
        {
            var commandResponses = command.Run();

            this.CommandExecuted?.Invoke(this, new CommandExecutedEventArgs(command));

            foreach (var x in commandResponses)
            {
                x.RedirectionTarget = command.RedirectionTarget;

                string destination;

                switch (x.Destination)
                {
                    case CommandResponseDestination.ChannelDebug:
                        destination = this.configProvider.DebugChannel;
                        break;
                    case CommandResponseDestination.PrivateMessage:
                        destination = command.User.Nickname;
                        break;
                    case CommandResponseDestination.Default:
                        // PMs to the bot.
                        destination = command.CommandSource == client.Nickname
                            ? command.User.Nickname
                            : command.CommandSource;
                        break;
                    default:
                        destination = command.CommandSource;
                        break;
                }

                           
                if (x.Type == CommandResponseType.Notice)
                {
                    client.Send(new Notice(destination, x.CompileMessage()));
                }
                else
                {
                    client.Send(new PrivateMessage(destination, x.CompileMessage()));
                }
            }
                
            globalStopwatch.Stop();
            this.logger.LogDebug(
                    "Command {CommandName} exec completed in {ElapsedMilliseconds}ms",
                    command.CommandName,
                    globalStopwatch.ElapsedMilliseconds);
        }
        finally
        {
            // wait 30 seconds for the post command events to finish execution, before finally killing the command
            command.CommandCompletedSemaphore.WaitOne(30000);
            this.commandParser.Release(command);
            this.logger.LogDebug("Command {CommandName} released", command.CommandName);
        }
    }
}