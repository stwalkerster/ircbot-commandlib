namespace Stwalkerster.Bot.CommandLib.Testbot.Command
{
    using System;
    using System.Collections.Generic;
    using Castle.Core.Logging;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Exceptions;
    using Stwalkerster.Bot.CommandLib.Model;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    [CommandInvocation("COI")]
    [CommandInvocation("BAR")]

    [CommandFlag("S")]
    public class OtherTestCommand : CommandBase
    {
        private readonly ICommandParser parser;

        public OtherTestCommand(
            string commandSource,
            IUser user,
            IList<string> arguments,
            ILogger logger,
            IFlagService flagService,
            IConfigurationProvider configurationProvider,
            IIrcClient client,
            ICommandParser parser) : base(
            commandSource,
            user,
            arguments,
            logger,
            flagService,
            configurationProvider,
            client)
        {
            this.parser = parser;
        }

        protected override IEnumerable<CommandResponse> OnPreRun(out bool abort)
        {
            abort = false;

            return null;
        }

        protected override IEnumerable<CommandResponse> Execute()
        {
            this.parser.GetCommandRegistrations();
            
            yield return new CommandResponse
            {
                Message = "Not a test, really?"
            };
        }
    }
}