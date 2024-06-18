namespace Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;

using System;
using Stwalkerster.Bot.CommandLib.Commands.Interfaces;

public class CommandExecutedEventArgs : EventArgs
{
    public CommandExecutedEventArgs(ICommand command)
    {
        this.Command = command;
    }

    public ICommand Command { get; }
}