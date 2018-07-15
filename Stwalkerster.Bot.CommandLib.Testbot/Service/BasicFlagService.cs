namespace Stwalkerster.Bot.CommandLib.Testbot.Service
{
    using System.Collections.Generic;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    public class BasicFlagService : IFlagService
    {
        public bool UserHasFlag(IUser user, string flag, string locality)
        {
            if (user.Nickname == "stwalkerster")
            {
                return true;
            }

            return flag == "S";
        }

        public IEnumerable<string> GetFlagsForUser(IUser user, string locality)
        {
            if (user.Nickname == "stwalkerster")
            {
                return new[] {"O", "S"};    
            }
            
            return new[] {"S"};
        }
    }
}