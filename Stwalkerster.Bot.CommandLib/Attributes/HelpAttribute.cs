namespace Stwalkerster.Bot.CommandLib.Attributes
{
    using System;
    using System.Collections.Generic;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;

    [AttributeUsage(AttributeTargets.Method)]
    public class HelpAttribute : Attribute
    {
        public HelpMessage HelpMessage { get; }

        public HelpAttribute(string syntax) : this(syntax, Array.Empty<string>())
        {
        }
        
        public HelpAttribute(string[] syntax) : this(syntax, Array.Empty<string>())
        {
        }
        
        public HelpAttribute(string syntax, string text)
            : this(new[] {syntax}, new[] {text})
        {
        }

        public HelpAttribute(string[] syntax, string text)
            : this(syntax, new[] {text})
        {
        }

        public HelpAttribute(string syntax, string[] text)
            : this(new[] {syntax}, text)
        {
        }

        public HelpAttribute(string[] syntax, string[] text)
        {
            this.HelpMessage = new HelpMessage(null, syntax, text);
        }
    }
}