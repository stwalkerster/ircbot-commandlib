namespace Stwalkerster.Bot.CommandLib.Exceptions;

using System;

[Serializable]
public class CommandInvocationException : CommandExecutionException
{
    public CommandInvocationException()
    {
    }

    public CommandInvocationException(string helpKey)
    {
        this.HelpKey = helpKey;
    }

    public string HelpKey { get; }
}