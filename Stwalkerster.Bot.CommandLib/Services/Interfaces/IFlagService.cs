﻿namespace Stwalkerster.Bot.CommandLib.Services.Interfaces;

using System.Collections.Generic;
using Stwalkerster.IrcClient.Model.Interfaces;

public interface IFlagService
{
    bool UserHasFlag(IUser user, string flag, string locality);
    IEnumerable<string> GetFlagsForUser(IUser user, string locality);
}