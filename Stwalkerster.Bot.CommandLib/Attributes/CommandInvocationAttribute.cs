namespace Stwalkerster.Bot.CommandLib.Attributes;

using System;
    
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class CommandInvocationAttribute(string commandName) : Attribute
{
    public string CommandName { get; } = commandName;
}