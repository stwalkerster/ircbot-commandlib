namespace Stwalkerster.Bot.CommandLib.Utilities;

using System.Collections.Generic;
using Stwalkerster.Bot.CommandLib.Model;
using Stwalkerster.Bot.CommandLib.Services.Interfaces;
using Stwalkerster.IrcClient.Interfaces;
using Stwalkerster.IrcClient.Model;
using Stwalkerster.IrcClient.Model.Interfaces;

public class BasicFlagService : IFlagService
{
    private readonly IrcUserMask ownerMask;
    public BasicFlagService(string ownerMask, IIrcClient client)
    {
        this.ownerMask = new IrcUserMask(ownerMask, client);
    }
    
    public bool UserHasFlag(IUser user, string flag, string locality)
    {
        if (this.ownerMask.Matches(user).GetValueOrDefault(false))
        {
            return true;
        }

        return flag == Flag.Standard;
    }

    public IEnumerable<string> GetFlagsForUser(IUser user, string locality)
    {
        if (this.ownerMask.Matches(user).GetValueOrDefault(false))
        {
            return new[] {Flag.Owner, Flag.Standard};
        }

        return new[] {Flag.Standard};
    }
}