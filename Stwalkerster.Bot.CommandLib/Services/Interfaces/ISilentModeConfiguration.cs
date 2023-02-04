namespace Stwalkerster.Bot.CommandLib.Services.Interfaces
{
    using Model;

    public interface ISilentModeConfiguration
    {
        bool BotIsSilent(string destination, CommandMessage message);
    }
}