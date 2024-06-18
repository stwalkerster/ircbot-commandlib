namespace Stwalkerster.Bot.CommandLib.Exceptions;

using System;

[Serializable]
public class CommandErrorException : CommandExecutionException
{
    public CommandErrorException(string message)
        : base(message)
    {
    }
    
    public CommandErrorException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}