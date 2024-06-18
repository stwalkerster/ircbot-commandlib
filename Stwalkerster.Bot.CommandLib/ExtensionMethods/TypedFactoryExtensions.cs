namespace Stwalkerster.Bot.CommandLib.ExtensionMethods;

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Stwalkerster.Bot.CommandLib.Commands.Interfaces;
using Stwalkerster.Bot.CommandLib.TypedFactories;
using Stwalkerster.IrcClient.Model.Interfaces;

public static class TypedFactoryExtensions
{
    public static ICommand CreateType(
        this ICommandTypedFactory factory,
        Type commandType,
        string commandSource,
        IUser user,
        IList<string> arguments,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger(commandType);

        return
            (ICommand)
            typeof(ICommandTypedFactory).GetMethod("Create")
                .MakeGenericMethod(commandType)
                .Invoke(factory, new object[] {commandSource, user, arguments, logger});
    }
}