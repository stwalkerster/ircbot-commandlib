namespace Stwalkerster.Bot.CommandLib.Testbot.Service
{
    using System.Collections.Generic;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    public class BasicFlagService : IFlagService
    {
        public bool UserHasFlag(IUser user, string flag) { return true; }
        public IEnumerable<string> GetFlagsForUser(IUser user) { return new[] {"A", "D", "P", "O", "B", "C"}; }
    }
}