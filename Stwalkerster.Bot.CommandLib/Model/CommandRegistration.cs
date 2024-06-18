namespace Stwalkerster.Bot.CommandLib.Model;

using System;

public class CommandRegistration
{
    public CommandRegistration(string channel, Type type)
    {
        this.Channel = channel;
        this.Type = type;
    }

    public string Channel { get; }

    public Type Type { get; }

    protected bool Equals(CommandRegistration other)
    {
        return string.Equals(this.Channel, other.Channel);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CommandRegistration) obj);
    }

    public override int GetHashCode()
    {
        return this.Channel != null ? this.Channel.GetHashCode() : 0;
    }
}