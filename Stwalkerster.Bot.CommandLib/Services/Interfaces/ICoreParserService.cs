namespace Stwalkerster.Bot.CommandLib.Services.Interfaces
{
    using Stwalkerster.Bot.CommandLib.Model;

    public interface ICoreParserService
    {
        CommandMessage ParseCommandMessage(string message, string nickname, string commandTrigger, bool isDirect, string initCharacter);
    }
}