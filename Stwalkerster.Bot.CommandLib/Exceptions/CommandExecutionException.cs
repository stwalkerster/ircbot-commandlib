namespace Stwalkerster.Bot.CommandLib.Exceptions;

using System;

[Serializable]
public abstract class CommandExecutionException : Exception
{
    protected CommandExecutionException(string message)
        : base(message)
    {
    }
    
    protected CommandExecutionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
    
    protected CommandExecutionException()
    {
    }
}