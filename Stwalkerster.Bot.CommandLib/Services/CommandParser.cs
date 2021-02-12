namespace Stwalkerster.Bot.CommandLib.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Castle.Core.Logging;
    using Prometheus;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
    using Stwalkerster.Bot.CommandLib.Commands.Interfaces;
    using Stwalkerster.Bot.CommandLib.ExtensionMethods;
    using Stwalkerster.Bot.CommandLib.Model;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.Bot.CommandLib.TypedFactories;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    /// <summary>
    /// The command parser.
    /// </summary>
    public class CommandParser : ICommandParser
    {
        private static readonly Gauge CommandCount = Metrics.CreateGauge(
            "irccommandlib_command_registrations",
            "Number of command registrations");
        
        #region Fields

        private readonly IConfigurationProvider configProvider;

        /// <summary>
        /// The command factory.
        /// </summary>
        private readonly ICommandTypedFactory commandFactory;

        private readonly ICoreParserService coreParserService;

        /// <summary>
        /// The command trigger.
        /// </summary>
        private readonly string commandTrigger;

        /// <summary>
        /// The commands.
        /// </summary>
        private readonly Dictionary<string, Dictionary<CommandRegistration, Type>> commands;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        #endregion

        #region Constructors and Destructors

        public CommandParser(
            IConfigurationProvider configProvider,
            ICommandTypedFactory commandFactory,
            ICoreParserService coreParserService,
            ILogger logger)
        {
            this.commandTrigger = configProvider.CommandPrefix;
            this.configProvider = configProvider;
            this.commandFactory = commandFactory;
            this.coreParserService = coreParserService;
            this.logger = logger;
            
            var types = new List<Type>(); 
            this.commands = new Dictionary<string, Dictionary<CommandRegistration, Type>>();
            
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                logger.DebugFormat("Scanning {0} for commands...", asm.FullName);
                
                foreach (var type in asm.GetTypes())
                {
                    if (!type.IsSubclassOf(typeof(CommandBase)))
                    {
                        // Not a new command class;
                        continue;
                    }

                    types.Add(type);
                }
            }
            
            foreach (var type in types)
            {
                var customAttributes = type.GetCustomAttributes(typeof(CommandInvocationAttribute), false);
                if (customAttributes.Length > 0)
                {
                    foreach (var attribute in customAttributes)
                    {
                        var commandName = ((CommandInvocationAttribute)attribute).CommandName;

                        if (commandName != string.Empty)
                        {
                            this.RegisterCommand(commandName, type);
                        }
                    }
                }
            }

            this.logger.InfoFormat("Initialised Command Parser with {0} commands.", this.commands.Count);
        }

        #endregion

        #region Public Methods and Operators

        public ICommand GetCommand(CommandMessage commandMessage, IUser user, string destination, IIrcClient client)
        {
            if (commandMessage?.CommandName == null)
            {
                this.logger.Debug("Returning early from GetCommand - null message!");
                return null;
            }

            IEnumerable<string> originalArguments = new List<string>();

            if (commandMessage.ArgumentList != null)
            {
                originalArguments =
                    commandMessage.ArgumentList.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            var redirectionResult = this.ParseRedirection(originalArguments);
            IList<string> arguments = redirectionResult.Arguments.ToList();

            var commandName = commandMessage.CommandName.ToLower(CultureInfo.InvariantCulture);
            var commandType = this.GetRegisteredCommand(commandName, destination);

            if (commandType != null)
            {   
                this.logger.InfoFormat("Creating command object of type {0}", commandType);

                try
                {
                    var command = this.commandFactory.CreateType(commandType, destination, user, arguments);

                    if (command is CommandBase runnable)
                    {
                        runnable.RedirectionTarget = redirectionResult.Target;
                        runnable.OriginalArguments = commandMessage.ArgumentList;
                        runnable.InvokedAs = commandName;
                    }

                    return command;
                }
                catch (TargetInvocationException e)
                {
                    this.logger.Error("Unable to create instance of command.", e.InnerException);
                    if (e.InnerException != null)
                    {
                        client.SendMessage(
                            this.configProvider.DebugChannel,
                            e.InnerException.Message.Replace("\r\n", " "));
                    }
                    else
                    {
                        client.SendMessage(
                            this.configProvider.DebugChannel,
                            "TargetInvocationException was raised in CommandParser, but no InnerException was present.");
                    }
                }
            }

            return null;
        }

        /// <inheritdoc />
        /// <summary>
        /// The parse redirection.
        /// </summary>
        /// <param name="inputArguments">
        /// The input arguments.
        /// </param>
        /// <returns>
        /// The <see cref="T:Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models.RedirectionResult" />.
        /// </returns>
        public RedirectionResult ParseRedirection(IEnumerable<string> inputArguments)
        {
            var targetList = new List<string>();
            var channelList = new List<string>();
            var parsedArguments = new List<string>();

            var redirecting = false;

            foreach (var argument in inputArguments)
            {
                if (redirecting)
                {
                    redirecting = false;
                    if (argument.StartsWith("#"))
                    {
                        channelList.Add(argument);
                    }
                    else
                    {
                        targetList.Add(argument);
                    }
                    
                    continue;
                }

                if (argument == ">")
                {
                    redirecting = true;
                    continue;
                }

                if (argument.StartsWith(">"))
                {
                    var arg = argument.Substring(1);

                    if (arg.StartsWith("#"))
                    {
                        channelList.Add(arg);
                    }
                    else
                    {
                        targetList.Add(arg);
                    }

                    continue;
                }

                parsedArguments.Add(argument);
            }

            // last word on line was >
            if (redirecting)
            {
                parsedArguments.Add(">");
            }

            return new RedirectionResult(parsedArguments, targetList, channelList);
        }
        
        public CommandMessage ParseCommandMessage(string message, string nickname)
        {
            return this.coreParserService.ParseCommandMessage(message, nickname, this.commandTrigger);
        }

        /// <summary>
        /// The release.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        public void Release(ICommand command)
        {
            this.commandFactory.Release(command);
        }

        public Dictionary<string, Dictionary<CommandRegistration, Type>> GetCommandRegistrations()
        {
            var result = new Dictionary<string, Dictionary<CommandRegistration, Type>>(this.commands.Count);

            foreach (var triggerEntry in this.commands)
            {
                var regTriggers = new Dictionary<CommandRegistration, Type>(triggerEntry.Value.Count);
                foreach (var regEntry in triggerEntry.Value)
                {
                    regTriggers.Add(
                        new CommandRegistration(regEntry.Key.Channel, regEntry.Key.Type),
                        regEntry.Key.Type);
                }

                result.Add(triggerEntry.Key, regTriggers);
            }

            return result;
        }

        #endregion

        #region Command Registration

        /// <summary>
        /// The register command.
        /// </summary>
        /// <param name="commandName">
        /// The keyword.
        /// </param>
        /// <param name="implementation">
        /// The implementation.
        /// </param>
        public void RegisterCommand(string commandName, Type implementation)
        {
            this.RegisterCommand(commandName, implementation, null);
        }

        /// <summary>
        /// The register command.
        /// </summary>
        /// <param name="commandName">
        /// The keyword.
        /// </param>
        /// <param name="implementation">
        /// The implementation.
        /// </param>
        /// <param name="channel">
        /// The channel to limit this registration to
        /// </param>
        public void RegisterCommand(string commandName, Type implementation, string channel)
        {
            if (!implementation.IsPublic)
            {
                this.logger.ErrorFormat(
                    "Implementation of command '{0}' ({1}) is not public, so cannot be instantiated! Refusing registration.",
                    commandName,
                    implementation);
                return;
            }

            if (!this.commands.ContainsKey(commandName))
            {
                this.commands.Add(commandName, new Dictionary<CommandRegistration, Type>());
            }

            this.commands[commandName].Add(new CommandRegistration(channel, implementation), implementation);
            this.logger.DebugFormat("Registered command {0}", implementation.FullName);
            
            CommandCount.Set(this.commands.Count);
        }

        public void UnregisterCommand(string commandName)
        {
            this.UnregisterCommand(commandName, null);
        }
        
        public void UnregisterCommand(string commandName, string channel)
        {
            if (!this.commands.ContainsKey(commandName))
            {
                return;
            }

            var cr = new CommandRegistration(channel, null);
            
            var registrations = this.commands[commandName];
            if (!registrations.ContainsKey(cr))
            {
                return;
            }

            registrations.Remove(cr);
            
            CommandCount.Set(this.commands.Count);
        }

        public Type GetRegisteredCommand(string commandName)
        {
            return this.GetRegisteredCommand(commandName, null);
        }
        
        public Type GetRegisteredCommand(string commandName, string destination)
        {
            if (!this.commands.TryGetValue(commandName, out var commandRegistrationSet))
            {
                // command doesn't exist anywhere
                return null;
            }

            if (destination != null)
            {
                var channelRegistration = commandRegistrationSet.Keys.FirstOrDefault(x => x.Channel == destination);

                if (channelRegistration != null)
                {
                    // This command is defined locally in this channel
                    return commandRegistrationSet[channelRegistration];
                }
            }

            var globalRegistration = commandRegistrationSet.Keys.FirstOrDefault(x => x.Channel == null);

            if (globalRegistration != null)
            {
                // This command is not defined locally, but is defined globally
                return commandRegistrationSet[globalRegistration];
            }

            // This command has a registration entry, but isn't defined in this channel or globally.
            return null;
        }

        #endregion

    }
}