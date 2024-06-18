namespace Stwalkerster.Bot.CommandLib.Exceptions;

using System;

[Serializable]
public class ConfigurationException : Exception
{
    public ConfigurationException(string message)
        : base(message)
    {
    }
}