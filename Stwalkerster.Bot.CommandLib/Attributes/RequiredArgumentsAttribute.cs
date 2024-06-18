namespace Stwalkerster.Bot.CommandLib.Attributes;

using System;
[AttributeUsage(AttributeTargets.Method)]
public class RequiredArgumentsAttribute : Attribute
{
    public RequiredArgumentsAttribute(int requiredArguments)
    {
        this.RequiredArguments = requiredArguments;
    }

    public int RequiredArguments { get; }
}