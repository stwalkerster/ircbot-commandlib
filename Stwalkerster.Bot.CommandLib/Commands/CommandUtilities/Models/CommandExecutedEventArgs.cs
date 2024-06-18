namespace Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;

using System;
using Stwalkerster.Bot.CommandLib.Commands.Interfaces;

public class CommandExecutedEventArgs(ICommand command) : EventArgs
{
    public ICommand Command { get; } = command;
}