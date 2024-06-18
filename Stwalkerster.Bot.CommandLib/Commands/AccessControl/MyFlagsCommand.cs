namespace Stwalkerster.Bot.CommandLib.Commands.AccessControl;

using System.Collections.Generic;
using Castle.Core.Logging;
using Stwalkerster.Bot.CommandLib.Attributes;
using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
using Stwalkerster.Bot.CommandLib.Services.Interfaces;
using Stwalkerster.IrcClient.Interfaces;
using Stwalkerster.IrcClient.Model.Interfaces;

[CommandInvocation("myflags")]
[CommandInvocation("myaccess")]
[CommandInvocation("whoami")]
[CommandFlag(Model.Flag.Standard)]
public class MyFlagsCommand : CommandBase
{
    public MyFlagsCommand(
        string commandSource, 
        IUser user, 
        IList<string> arguments, 
        ILogger logger,  
        IFlagService flagService,
        IConfigurationProvider configurationProvider,
        IIrcClient client)
        : base(commandSource, user, arguments, logger, flagService, configurationProvider, client)
    {
    }
    
    protected override IEnumerable<CommandResponse> Execute()
    {
        var flagsForUser = this.FlagService.GetFlagsForUser(this.User, this.CommandSource);

        var message = string.Format(
            "The flags currently available to {0}{2} are: {1}",
            this.User,
            string.Join(string.Empty, flagsForUser),
            this.CommandSource.StartsWith("#") ? " in " + this.CommandSource : "");

        yield return new CommandResponse { Message = message };
    }
    
    protected override IDictionary<string, HelpMessage> Help()
    {
        return new Dictionary<string, HelpMessage>
        {
            {
                string.Empty,
                new HelpMessage(
                    this.CommandName,
                    string.Empty,
                    "Retrieves the flags available to the current user.")
            }
        };
    }
}