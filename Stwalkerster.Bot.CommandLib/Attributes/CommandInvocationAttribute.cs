namespace Stwalkerster.Bot.CommandLib.Attributes
{
    using System;
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class CommandInvocationAttribute : Attribute
    {
        public CommandInvocationAttribute(string commandName)
        {
            this.CommandName = commandName;
        }

        public string CommandName { get; }
    }
}