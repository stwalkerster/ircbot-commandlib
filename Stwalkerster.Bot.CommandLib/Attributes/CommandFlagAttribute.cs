namespace Stwalkerster.Bot.CommandLib.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class CommandFlagAttribute : Attribute
    {
        public CommandFlagAttribute(string flag)
        {
            this.Flag = flag;
            this.GlobalOnly = false;
        }
        
        public CommandFlagAttribute(string flag, bool globalOnly)
        {
            this.Flag = flag;
            this.GlobalOnly = globalOnly;
        }
       
        public string Flag { get; }
        public bool GlobalOnly { get; }
    }
}