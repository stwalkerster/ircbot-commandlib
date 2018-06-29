namespace Stwalkerster.Bot.CommandLib.Testbot.Service
{
    using System.Collections.Generic;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    public class BasicFlagService : IFlagService
    {
        public bool UserHasFlag(IUser user, string flag, string locality) { return true; }
        public IEnumerable<string> GetFlagsForUser(IUser user, string locality) { return new[] {"O", "S"}; }
    }
}