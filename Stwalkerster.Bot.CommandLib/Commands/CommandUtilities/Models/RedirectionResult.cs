namespace Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models
{
    using System.Collections.Generic;

    public class RedirectionResult
    {
        public RedirectionResult(IEnumerable<string> arguments, IEnumerable<string> target, IEnumerable<string> channelTargets)
        {
            this.Arguments = arguments;
            this.Target = target;
            this.ChannelTargets = channelTargets;
        }

        public IEnumerable<string> Arguments { get; }

        public IEnumerable<string> Target { get; }

        public IEnumerable<string> ChannelTargets { get; }
    }
}