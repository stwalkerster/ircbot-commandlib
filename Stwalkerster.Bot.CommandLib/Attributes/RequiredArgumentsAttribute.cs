namespace Stwalkerster.Bot.CommandLib.Attributes;

using System;
[AttributeUsage(AttributeTargets.Method)]
public class RequiredArgumentsAttribute(int requiredArguments) : Attribute
{
    public int RequiredArguments { get; } = requiredArguments;
}