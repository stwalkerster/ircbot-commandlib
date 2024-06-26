﻿namespace Stwalkerster.Bot.CommandLib.Services.Interfaces;

public interface IConfigurationProvider
{
    string CommandPrefix { get; }
    string DebugChannel { get; }
    bool AllowQuotedStrings { get; }
    bool IncludeBuiltins { get; }
    string UseCommandInitSeparator { get; }
}