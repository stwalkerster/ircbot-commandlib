namespace Stwalkerster.Bot.CommandLib.Model;

public class CommandMessage
{
    public CommandMessage(string commandName)
    {
        this.ArgumentList = string.Empty;
        this.CommandName = commandName;
        this.OverrideSilence = false;
    }

    public CommandMessage(string commandName, string argumentList, bool overrideSilence)
    {
        this.ArgumentList = argumentList;
        this.CommandName = commandName;
        this.OverrideSilence = overrideSilence;
    }

    public string ArgumentList { get; }
    public string CommandName { get; }
    public bool OverrideSilence { get; }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return this.Equals((CommandMessage) obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = this.ArgumentList != null ? this.ArgumentList.GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ (this.CommandName != null ? this.CommandName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ this.OverrideSilence.GetHashCode();
            return hashCode;
        }
    }

    public override string ToString()
    {
        return
            $"ArgumentList: {this.ArgumentList}, CommandName: {this.CommandName}, OverrideSilence: {this.OverrideSilence}";
    }

    protected bool Equals(CommandMessage other)
    {
        return string.Equals(this.ArgumentList, other.ArgumentList)
               && string.Equals(this.CommandName, other.CommandName)
               && this.OverrideSilence.Equals(other.OverrideSilence);
    }
}