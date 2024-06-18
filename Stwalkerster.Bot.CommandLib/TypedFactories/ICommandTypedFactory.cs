namespace Stwalkerster.Bot.CommandLib.TypedFactories;

using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Stwalkerster.Bot.CommandLib.Commands.Interfaces;
using Stwalkerster.IrcClient.Model.Interfaces;

public interface ICommandTypedFactory
{
    T Create<T>(string commandSource, IUser user, IList<string> arguments, ILogger logger)
        where T : ICommand;

    void Release(ICommand command);
}