namespace Stwalkerster.Bot.CommandLib.Testbot.Command
{
    using System.Collections.Generic;
    using Castle.Core.Logging;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    [CommandInvocation("test")]
    [CommandInvocation("foo")]
    [CommandFlag("B")]
    public class TestCommand : CommandBase
    {
        public TestCommand(string commandSource, IUser user, IEnumerable<string> arguments, ILogger logger, IFlagService flagService, IConfigurationProvider configurationProvider, IIrcClient client) : base(commandSource, user, arguments, logger, flagService, configurationProvider, client)
        {
        }

        protected override IEnumerable<CommandResponse> Execute()
        {
            yield return new CommandResponse
            {
                Message = "Ohai there. Invoked as " + this.InvokedAs
            };
        }
    }
}