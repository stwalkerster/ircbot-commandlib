namespace Stwalkerster.Bot.CommandLib.Testbot.Service
{
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    public class ConfigProvider : IConfigurationProvider
    {
        public string CommandPrefix { get { return "!"; } }
        public string DebugChannel { get { return "##stwalkerster-development"; } }
        public bool AllowQuotedStrings => true;
    }
}