namespace Stwalkerster.Bot.CommandLib.Testbot.Service
{
    using Castle.Core;
    using Castle.Core.Logging;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;

    public class AccessReporterService : IStartable
    {
        private readonly ILogger logger;

        public AccessReporterService(ILogger logger, ICommandHandler commandHandler)
        {
            this.logger = logger;
            
            commandHandler.CommandExecuted += this.OnCommandExecuted;
        }

        private void OnCommandExecuted(object sender, CommandExecutedEventArgs args)
        {
            this.logger.InfoFormat("Command {0} invoked, result {1}", args.Command.InvokedAs, args.Command.ExecutionStatus.AclStatus);
        }

        public void Start()
        {
            this.logger.Info("Access reporter started");
        }

        public void Stop()
        {
            
        }
    }
}