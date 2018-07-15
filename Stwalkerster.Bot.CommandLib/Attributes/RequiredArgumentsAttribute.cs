namespace Stwalkerster.Bot.CommandLib.Attributes
{
    using System;
    [AttributeUsage(AttributeTargets.Method)]
    public class RequiredArgumentsAttribute : Attribute
    {
        public int RequiredArguments { get; }

        public RequiredArgumentsAttribute(int requiredArguments)
        {
            this.RequiredArguments = requiredArguments;
        }
    }
}