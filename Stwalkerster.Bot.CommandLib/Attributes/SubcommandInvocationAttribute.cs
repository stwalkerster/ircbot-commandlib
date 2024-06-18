namespace Stwalkerster.Bot.CommandLib.Attributes;

using System;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class SubcommandInvocationAttribute : Attribute
{
    public SubcommandInvocationAttribute(string commandName)
    {
        this.CommandName = commandName;
    }

    public string CommandName { get; }
}