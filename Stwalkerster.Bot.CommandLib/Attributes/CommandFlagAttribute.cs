namespace Stwalkerster.Bot.CommandLib.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CommandFlagAttribute : Attribute
    {
        public CommandFlagAttribute(string flag)
        {
            this.Flag = flag;
        }

        public string Flag { get; }
    }
}