namespace Stwalkerster.Bot.CommandLib.Exceptions
{
    using System;

    [Serializable]
    public class CommandAccessDeniedException : CommandExecutionException
    {
        public CommandAccessDeniedException()
        {
        }
        
        public CommandAccessDeniedException(string helpKey)
        {
            this.HelpKey = helpKey;
        }
        
        public string HelpKey { get; }
    }
}