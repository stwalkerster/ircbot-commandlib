﻿namespace Stwalkerster.Bot.CommandLib.Services;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
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

public class CommandParser : ICommandParser
{
    private static readonly Gauge CommandCount = Metrics.CreateGauge(
        "irccommandlib_command_registrations",
        "Number of command registrations");
    
    private readonly IConfigurationProvider configProvider;
    private readonly ICommandTypedFactory commandFactory;
    private readonly ICoreParserService coreParserService;
    private readonly string commandTrigger;
    private readonly Dictionary<string, Dictionary<CommandRegistration, Type>> commands;
    private readonly ILogger<CommandParser> logger;
    private readonly ILoggerFactory loggerFactory;

    public CommandParser(
        IConfigurationProvider configProvider,
        ICommandTypedFactory commandFactory,
        ICoreParserService coreParserService,
        ILogger<CommandParser> logger,
        ILoggerFactory loggerFactory)
    {
        this.commandTrigger = configProvider.CommandPrefix;
        this.configProvider = configProvider;
        this.commandFactory = commandFactory;
        this.coreParserService = coreParserService;
        this.logger = logger;
        this.loggerFactory = loggerFactory;

        var types = new List<Type>(); 
        this.commands = new Dictionary<string, Dictionary<CommandRegistration, Type>>();
        
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            logger.LogDebug("Scanning {Assembly} for commands...", asm.FullName);

            if (Assembly.GetExecutingAssembly().Equals(asm) && !configProvider.IncludeBuiltins)
            {
                continue;
            }
            
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

        this.logger.LogInformation("Initialised Command Parser with {CommandCount} commands", this.commands.Count);
    }
    
    public ICommand GetCommand(CommandMessage commandMessage, IUser user, string destination, IIrcClient client)
    {
        if (commandMessage?.CommandName == null)
        {
            this.logger.LogDebug("Returning early from GetCommand - null message!");
            return null;
        }

        IEnumerable<string> originalArguments = new List<string>();

        if (commandMessage.ArgumentList != null)
        {
            if (this.configProvider.AllowQuotedStrings)
            {
                var r = new Regex(@"(?:"".*?""|\S)+");
                var matchCollection = r.Matches(commandMessage.ArgumentList);

                var args = (from Match m in matchCollection select m.Value).ToList();
                originalArguments = args;
            }
            else
            {
                originalArguments =
                    commandMessage.ArgumentList.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();    
            }
        }

        var redirectionResult = this.ParseRedirection(originalArguments);
        IList<string> arguments = redirectionResult.Arguments.ToList();

        var commandName = commandMessage.CommandName;
        var commandType = this.GetRegisteredCommand(commandName, destination);

        if (commandType == null)
        {
            // no exact match try munging to lower
            commandName = commandName.ToLower(CultureInfo.InvariantCulture);
            commandType = this.GetRegisteredCommand(commandName, destination);
        }
        
        if (commandType == null)
        {
            // no exact match or lowercase match, try munging to upper
            commandName = commandName.ToUpper(CultureInfo.InvariantCulture);
            commandType = this.GetRegisteredCommand(commandName, destination);
        }
        
        if (commandType != null)
        {   
            this.logger.LogInformation("Creating command object of type {Type}", commandType);

            try
            {
                var command = this.commandFactory.CreateType(commandType, destination, user, arguments, this.loggerFactory);

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
                this.logger.LogError(e.InnerException, "Unable to create instance of command");
                
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

        // last word on the line was >
        if (redirecting)
        {
            parsedArguments.Add(">");
        }

        return new RedirectionResult(parsedArguments, targetList, channelList);
    }
    
    public CommandMessage ParseCommandMessage(string message, string nickname, bool isDirect)
    {
        return this.coreParserService.ParseCommandMessage(message, nickname, this.commandTrigger, isDirect, this.configProvider.UseCommandInitSeparator);
    }

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
    
    public void RegisterCommand(string commandName, Type implementation)
    {
        this.RegisterCommand(commandName, implementation, null);
    }

    public void RegisterCommand(string commandName, Type implementation, string channel)
    {
        if (!implementation.IsPublic)
        {
            this.logger.LogError(
                "Implementation of command '{CommandName}' ({Implementation}) is not public, so cannot be instantiated - refusing registration",
                commandName,
                implementation);
            return;
        }

        if (!this.commands.ContainsKey(commandName))
        {
            this.commands.Add(commandName, new Dictionary<CommandRegistration, Type>());
        }

        this.commands[commandName].Add(new CommandRegistration(channel, implementation), implementation);
        this.logger.LogDebug("Registered command {Implementation}", implementation.FullName);
        
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
}