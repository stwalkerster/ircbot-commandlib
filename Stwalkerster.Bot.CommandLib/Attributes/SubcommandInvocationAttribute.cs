namespace Stwalkerster.Bot.CommandLib.Attributes;

using System;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class SubcommandInvocationAttribute(string commandName) : Attribute
{
    public string CommandName { get; } = commandName;
}