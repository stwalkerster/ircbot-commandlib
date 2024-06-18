﻿namespace Stwalkerster.Bot.CommandLib.ExtensionMethods;

using System;
using System.Collections.Generic;
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
        IList<string> arguments)
    {
        return
            (ICommand)
            typeof(ICommandTypedFactory).GetMethod("Create")
                .MakeGenericMethod(commandType)
                .Invoke(factory, [commandSource, user, arguments]);
    }
}