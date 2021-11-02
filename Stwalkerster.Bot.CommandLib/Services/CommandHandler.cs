namespace Stwalkerster.Bot.CommandLib.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Events;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Messages;

    /// <summary>
    /// The command handler.
    /// </summary>
    public class CommandHandler : ICommandHandler
    {
        #region Fields

        /// <summary>
        /// The command parser.
        /// </summary>
        private readonly ICommandParser commandParser;

        private readonly IConfigurationProvider configProvider;

        #endregion

        public event EventHandler<CommandExecutedEventArgs> CommandExecuted;

        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="CommandHandler"/> class.
        /// </summary>
        /// <param name="commandParser">
        /// The command parser.
        /// </param>
        /// <param name="configProvider"></param>
        public CommandHandler(ICommandParser commandParser, IConfigurationProvider configProvider)
        {
            this.commandParser = commandParser;
            this.configProvider = configProvider;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Called on new messages received by the IRC client
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (e.IsNotice)
            {
                return;
            }

            ThreadPool.QueueUserWorkItem(this.ProcessMessageAsync, e);
        }

        /// <summary>

        /// The process message async.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        private void ProcessMessageAsync(object state)
        {
            var eventArgs = (MessageReceivedEventArgs)state;

            IIrcClient client = eventArgs.Client;

            string message = eventArgs.Message;

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
                return;
            }

            try
            {
                IEnumerable<CommandResponse> commandResponses = command.Run();

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
                            if (command.CommandSource == client.Nickname)
                            {
                                // PMs to the bot.
                                destination = command.User.Nickname;
                            }
                            else
                            {
                                destination = command.CommandSource;
                            }
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
            }
            finally
            {
                // wait 30 seconds for the post command events to finish execution, before finally killing the command
                command.CommandCompletedSemaphore.WaitOne(30000);
                this.commandParser.Release(command);
            }
        }

        #endregion
    }
}