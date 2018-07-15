namespace Stwalkerster.Bot.CommandLib.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core.Logging;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Exceptions;
    using Stwalkerster.Bot.CommandLib.Model;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    /// <summary>
    /// The help command.
    /// </summary>
    [CommandFlag(Flag.Standard)]
    [CommandInvocation("help")]
    public class HelpCommand : CommandBase
    {
        private readonly ICommandParser commandParser;

        public HelpCommand(
            string commandSource, 
            IUser user, 
            IList<string> arguments, 
            ILogger logger,  
            IFlagService flagService,
            IConfigurationProvider configurationProvider,
            ICommandParser commandParser,
            IIrcClient client)
            : base(commandSource, user, arguments, logger, flagService, configurationProvider, client)
        {
            this.commandParser = commandParser;
        }

        protected override IEnumerable<CommandResponse> Execute()
        {
            if (!this.Arguments.Any())
            {
                throw new ArgumentCountException();
            }

            string commandName = this.Arguments.ElementAt(0);
            string key = this.Arguments.Count > 1 ? this.Arguments.ElementAt(1) : null;

            var command = this.commandParser.GetCommand(
                new CommandMessage ( commandName ), 
                this.User, 
                this.CommandSource,
                this.Client);

            if (command == null)
            {
                return new List<CommandResponse>
                {
                    new CommandResponse
                    {
                        Message = "The specified command could not be found.",
                        Destination = CommandResponseDestination.PrivateMessage
                    }
                };
            }

            var helpResponses = command.HelpMessage(key).ToList();

            return helpResponses;
        }

        protected override IDictionary<string, HelpMessage> Help()
        {
            return new Dictionary<string, HelpMessage>
                       {
                           {
                               string.Empty, 
                               new HelpMessage(
                               this.CommandName, 
                               "<Command>", 
                               "Returns all available help for the specified command.")
                           }, 
                           {
                               "command", 
                               new HelpMessage(
                               this.CommandName, 
                               "<Command> <SubCommand>", 
                               "Returns the help for the specified subcommand.")
                           }
                       };
        }
    }
}