namespace Stwalkerster.Bot.CommandLib.Testbot.Command
{
    using System.Collections.Generic;
    using Castle.Core.Logging;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Model;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    [CommandInvocation("test")]
    [CommandInvocation("foo")]
    [CommandFlag("S")]
    public class TestCommand : CommandBase
    {
        public TestCommand(
            string commandSource,
            IUser user,
            IList<string> arguments,
            ILogger logger,
            IFlagService flagService,
            IConfigurationProvider configurationProvider,
            IIrcClient client) : base(
            commandSource,
            user,
            arguments,
            logger,
            flagService,
            configurationProvider,
            client)
        {
        }

       // [Help("", "Says hi to the user")]
        protected override IEnumerable<CommandResponse> Execute()
        {
            yield return new CommandResponse
            {
                Message = "Ohai there " + this.User + " in " + this.CommandSource
            };
        }

        [Help(new[] {"<user>"}, new[] {"says bye angrilly"})]
        [SubcommandInvocation("bye")]
        [CommandFlag(Flag.Owner)]
        [RequiredArguments(2)]
        protected IEnumerable<CommandResponse> RunBye()
        {
            yield return new CommandResponse
            {
                Message = "BYE THEN " + this.User + "!"
            };
        }
    }
}