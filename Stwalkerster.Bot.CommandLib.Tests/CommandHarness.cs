namespace Stwalkerster.Bot.CommandLib.Tests;

using System;
using System.Collections.Generic;
using Castle.Core.Logging;
using CommandLib.Commands.CommandUtilities;
using CommandLib.Commands.CommandUtilities.Response;
using CommandLib.Services.Interfaces;
using IrcClient.Interfaces;
using IrcClient.Model.Interfaces;

public class CommandHarness : CommandBase
{
    public CommandHarness(string commandSource, IUser user, IList<string> arguments, ILogger logger, IFlagService flagService, IConfigurationProvider configurationProvider, IIrcClient client) : base(commandSource, user, arguments, logger, flagService, configurationProvider, client)
    {
    }

    public virtual Func<IEnumerable<CommandResponse>> OnExecute { get; } = () => new List<CommandResponse>();
    public int DoneCompleted { get; private set; } = 0;
    public int DoneAccessDenied { get; private set; } = 0;
    
    protected override IEnumerable<CommandResponse> Execute()
    {
        return this.OnExecute();
    }

    protected override IEnumerable<CommandResponse> OnCompleted()
    {
        this.DoneCompleted++;
        return base.OnCompleted();
    }

    protected override IEnumerable<CommandResponse> OnAccessDenied()
    {
        this.DoneAccessDenied++;
        return base.OnAccessDenied();
    }
}