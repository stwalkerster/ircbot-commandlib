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

    [CommandInvocation("test")]
    [CommandInvocation("foo")]
    [CommandInvocation("coi")]
    [CommandInvocation("💩")]
    
    [CommandFlag("S")]
    public class TestCommand : CommandBase
    {
        private readonly ICommandParser parser;

        public TestCommand(
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

        [CommandParameter("f|foo", "Foo", "foo", typeof(bool))]
        [CommandParameter("bar", "Foo", "bar", typeof(bool))]
        [CommandParameter("baz", "Foo", "baz", typeof(bool))]
        protected override IEnumerable<CommandResponse> Execute()
        {
            this.parser.GetCommandRegistrations();
            
            yield return new CommandResponse
            {
                Message = "Ohai there " + this.User + " in " + this.CommandSource
            };
        }

        [SubcommandInvocation("bye")]
        [CommandFlag("A", true)]
        [CommandFlag("B")]
        [RequiredArguments(1)]
        [CommandParameter("g|f|foo", "Foo", "foo", typeof(bool))]
        protected IEnumerable<CommandResponse> RunBye()
        {
            yield return new CommandResponse
            {
                Message = "BYE THEN " + this.User + "!"
            };
        }
    }
}