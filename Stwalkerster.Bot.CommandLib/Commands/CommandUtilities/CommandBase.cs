﻿namespace Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.Logging;
using Mono.Options;
using Prometheus;
using Stwalkerster.Bot.CommandLib.Attributes;
using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
using Stwalkerster.Bot.CommandLib.Commands.Interfaces;
using Stwalkerster.Bot.CommandLib.Exceptions;
using Stwalkerster.Bot.CommandLib.ExtensionMethods;
using Stwalkerster.Bot.CommandLib.Model;
using Stwalkerster.Bot.CommandLib.Services.Interfaces;
using Stwalkerster.IrcClient.Interfaces;
using Stwalkerster.IrcClient.Model.Interfaces;

public abstract class CommandBase : ICommand
{
    private static readonly Counter CommandExecutions = Metrics.CreateCounter(
        "irccommandlib_command_executions_total",
        "Number of command executions",
        new CounterConfiguration { LabelNames = new[] { "result" } });

    private readonly IConfigurationProvider configurationProvider;

    protected CommandBase(
        string commandSource,
        IUser user,
        IList<string> arguments,
        ILogger logger,
        IFlagService flagService,
        IConfigurationProvider configurationProvider,
        IIrcClient client)
    {
        this.FlagService = flagService;
        this.configurationProvider = configurationProvider;
        this.CommandCompletedSemaphore = new Semaphore(1, 1);
        this.Logger = logger;
        this.Client = client;
        this.CommandSource = commandSource;
        this.User = user;
        this.Arguments = arguments;
        this.Parameters = new Dictionary<string, object>();

        this.ExecutionStatus = new CommandExecutionStatus();
    }

    public bool Executed { get; private set; }
    public CommandExecutionStatus ExecutionStatus { get; private set; }

    /// <inheritdoc />
    public Semaphore CommandCompletedSemaphore { get; }

    /// <summary>
    /// Returns the collection of arguments passed to this command
    /// </summary>
    public IList<string> Arguments { get; }

    /// <summary>
    /// Returns the values of any provided named parameters
    /// </summary>
    public IDictionary<string, object> Parameters { get; }

    /// <summary>
    /// Returns the OptionSet of recognised parameters for this subcommand.
    /// </summary>
    public OptionSet OptionSet { get; private set; }

    /// <summary>
    /// Returns the name under which this command was invoked
    /// </summary>
    public string InvokedAs { get; internal set; }

    /// <summary>
    /// Returns the subcommand which was invoked
    /// </summary>
    public string SubCommand { get; internal set; }

    protected IFlagService FlagService { get; }

    /// <summary>
    /// Returns the canonical name of this command
    /// </summary>
    public string CommandName
    {
        get
        {
            var customAttributes = this.GetType().GetCustomAttributes(typeof(CommandInvocationAttribute), false);
            if (customAttributes.Length > 0)
            {
                return ((CommandInvocationAttribute)customAttributes.First()).CommandName;
            }

            return null;
        }
    }

    /// <inheritdoc />
    public string CommandSource { get; }

    /// <summary>
    /// Gets the original string of data which was passed to this command as an argument.
    /// </summary>
    public string OriginalArguments { get; internal set; }

    /// <inheritdoc />
    public IEnumerable<string> RedirectionTarget { get; internal set; }

    /// <inheritdoc />
    public IUser User { get; }

    protected ILogger Logger { get; }
    public IIrcClient Client { get; }

    protected virtual IEnumerable<CommandResponse> Execute()
    {
        throw new CommandInvocationException();
    }

    protected virtual IEnumerable<CommandResponse> OnPreRun(out bool abort)
    {
        abort = false;
        return null;
    }

    public IEnumerable<CommandResponse> Run()
    {
        if (this.Executed)
        {
            throw new Exception("Already executed instance of command");
        }

        try
        {
            // Test global access for this command
            if (!this.AllowedMainCommand())
            {
                this.Logger.InfoFormat("Access denied command-globally for user {0}", this.User);

                var accessDeniedResponses = this.OnAccessDenied() ?? new List<CommandResponse>();
                this.ExecutionStatus.AclStatus = CommandAclStatus.DeniedMain;
                CommandExecutions.WithLabels("globally-denied").Inc();
                return accessDeniedResponses;
            }

            var subCommandMethod = this.GetSubCommandMethod();

            if (subCommandMethod == null)
            {
                this.Logger.WarnFormat(
                    "Unable to locate an executable method for command {0} with args: {1}",
                    this.CommandName,
                    this.OriginalArguments);

                var missingSubcommandResponses = this.OnMissingSubcommand() ?? new List<CommandResponse>();
                CommandExecutions.WithLabels("no-subcommand").Inc();
                return missingSubcommandResponses;
            }

            // Test local access for this command
            if (!this.AllowedSubcommand(subCommandMethod))
            {
                this.Logger.InfoFormat("Access denied subcommand-locally for user {0}", this.User);

                var accessDeniedResponses = this.OnAccessDenied() ?? new List<CommandResponse>();
                this.ExecutionStatus.AclStatus = CommandAclStatus.DeniedSubcommand;
                CommandExecutions.WithLabels("subcommand-denied").Inc();
                return accessDeniedResponses;
            }

            this.OptionSet = subCommandMethod.ParseOptionSet(this.OptionSetParseBool, this.OptionSetParseString);

            if (!this.ParseParameters(out var parameterResponse))
            {
                return parameterResponse;
            }

            if (!this.ValidateArgumentCount(subCommandMethod, out var response))
            {
                return response;
            }

            try
            {
                IEnumerable<CommandResponse> preRunResponses;
                bool abort;

                try
                {
                    preRunResponses = this.OnPreRun(out abort) ?? new List<CommandResponse>();
                }
                catch (CommandAccessDeniedException)
                {
                    this.Logger.InfoFormat("Access denied on pre-run for user {0}", this.User);

                    var accessDeniedResponses = this.OnAccessDenied() ?? new List<CommandResponse>();
                    this.ExecutionStatus.AclStatus = CommandAclStatus.DeniedPrerun;
                    CommandExecutions.WithLabels("prerun-denied").Inc();
                    return accessDeniedResponses;
                }

                if (abort)
                {
                    return preRunResponses;
                }

                this.ExecutionStatus.AclStatus = CommandAclStatus.Allowed;

                IEnumerable<CommandResponse> commandResponses;
                try
                {
                    commandResponses = (IEnumerable<CommandResponse>)subCommandMethod.Invoke(this, null);
                    CommandExecutions.WithLabels("allowed").Inc();
                }
                catch (Exception e) when (
                    (e is TargetInvocationException && e.InnerException is CommandAccessDeniedException)
                    || e is CommandAccessDeniedException)
                {
                    this.Logger.InfoFormat("Access denied during command execution for user {0}", this.User);

                    var accessDeniedResponses = this.OnAccessDenied() ?? new List<CommandResponse>();
                    this.ExecutionStatus.AclStatus = CommandAclStatus.DeniedRuntime;
                    CommandExecutions.WithLabels("runtime-denied").Inc();
                    return accessDeniedResponses;
                }

                commandResponses = commandResponses ?? new List<CommandResponse>();

                var completedResponses = this.OnCompleted() ?? new List<CommandResponse>();

                // Resolve the list into a concrete list before committing the transaction.
                var responses = preRunResponses.Concat(commandResponses.Concat(completedResponses)).ToList();

                return responses;
            }
            catch (Exception e) when (
                (e is TargetInvocationException && e.InnerException is CommandInvocationException)
                || e is CommandInvocationException)
            {
                this.Logger.Info("Command encountered an issue from invocation.");

                if (e is TargetInvocationException)
                {
                    e = e.InnerException;
                }

                return this.HelpMessage(((CommandInvocationException)e).HelpKey);
            }
            catch (Exception e) when (
                (e is TargetInvocationException && e.InnerException is ArgumentCountException)
                || e is ArgumentCountException)
            {
                if (e is TargetInvocationException)
                {
                    e = e.InnerException;
                }

                return this.AlertArgumentCount((ArgumentCountException)e);
            }
            catch (Exception e) when (
                (e is TargetInvocationException && e.InnerException is CommandExecutionException)
                || e is CommandExecutionException)
            {
                if (e is TargetInvocationException)
                {
                    e = e.InnerException;
                }

                this.Logger.Warn("Command encountered an issue during execution.", e);

                return new List<CommandResponse>
                {
                    new CommandResponse
                    {
                        Destination = CommandResponseDestination.Default,
                        Message = e.Message
                    }
                };
            }
            catch (Exception e)
            {
                this.Logger.Error("Unhandled exception during command execution", e);

                return new List<CommandResponse>
                {
                    new CommandResponse
                    {
                        Destination = CommandResponseDestination.Default,
                        Message = "Unhandled exception during command execution."
                    }
                };
            }
        }
        finally
        {
            this.Executed = true;
        }
    }

    private bool ValidateArgumentCount(MethodInfo info, out IEnumerable<CommandResponse> response)
    {
        var attr = info.GetAttribute<RequiredArgumentsAttribute>();
        if (attr == null)
        {
            response = null;
            return true;
        }

        if (attr.RequiredArguments <= this.Arguments.Count)
        {
            response = null;
            return true;
        }

        response = this.AlertArgumentCount(
            new ArgumentCountException(attr.RequiredArguments, this.Arguments.Count));
        return false;
    }

    private IEnumerable<CommandResponse> AlertArgumentCount(ArgumentCountException e)
    {
        this.Logger.Info("Command executed with missing arguments.");

        var responses = new List<CommandResponse>
        {
            new CommandResponse
            {
                Destination = CommandResponseDestination.Default,
                Message = e.Message
            }
        };

        responses.AddRange(this.HelpMessage(e.HelpKey));

        return responses;
    }

    private MethodInfo GetSubCommandMethod()
    {
        if (!this.Arguments.Any())
        {
            var subCommandMethod = this.GetType()
                .GetMethod("Execute", BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic);
            return subCommandMethod;
        }

        var methodInfos = this.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic);

        foreach (var info in methodInfos)
        {
            this.Logger.DebugFormat("Found method: {0}", info.Name);

            if (info.IsAbstract || info.IsConstructor || info.IsPrivate)
            {
                continue;
            }

            var attrs = info.GetAttributes<SubcommandInvocationAttribute>();

            var attr = attrs?.FirstOrDefault(x => x.CommandName.ToLower() == this.Arguments.First().ToLower());
            if (attr == null)
            {
                continue;
            }

            if (info.ReturnType != typeof(IEnumerable<CommandResponse>))
            {
                this.Logger.Error("Found subcommand, but subcommand return type is wrong.");
                break;
            }

            if (info.GetParameters().Any())
            {
                this.Logger.Error("Found subcommand, but subcommand has parameters.");
                break;
            }

            this.Logger.DebugFormat(
                "Invoking method {0} as subcommand for command {1}",
                info.Name,
                this.InvokedAs);

            this.SubCommand = attr.CommandName.ToLower();
            this.Arguments.RemoveAt(0);

            return info;
        }

        return this.GetType()
            .GetMethod("Execute", BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic);
    }

    private bool AllowedMainCommand()
    {
        var flagAttributes = this.GetType().GetAttributes<CommandFlagAttribute>().ToList();

        this.ExecutionStatus.MainFlags = flagAttributes.Aggregate(
            string.Empty,
            (seed, item) => seed + item.Flag + (item.GlobalOnly ? "*" : string.Empty));

        foreach (var attribute in flagAttributes)
        {
            var result = this.FlagService.UserHasFlag(
                this.User,
                attribute.Flag,
                attribute.GlobalOnly ? null : this.CommandSource);

            if (result)
            {
                return true;
            }
        }

        if (!flagAttributes.Any())
        {
            this.Logger.Warn(
                "Access to command was assumed to be owner-level because there are no flags set for this command.");
            return this.FlagService.UserHasFlag(this.User, Flag.Owner, this.CommandSource);
        }

        return false;
    }

    private bool AllowedSubcommand(MethodInfo info)
    {
        var flagAttributes = info.GetAttributes<CommandFlagAttribute>().ToList();

        this.ExecutionStatus.SubcommandFlags = flagAttributes.Aggregate(
            string.Empty,
            (seed, item) => seed + item.Flag + (item.GlobalOnly ? "*" : string.Empty));

        foreach (var attribute in flagAttributes)
        {
            var result = this.FlagService.UserHasFlag(
                this.User,
                attribute.Flag,
                attribute.GlobalOnly ? null : this.CommandSource);

            if (result)
            {
                return true;
            }
        }

        if (!flagAttributes.Any())
        {
            return this.AllowedMainCommand();
        }

        return false;
    }

    private void OptionSetParseBool(string x, string resultName, Func<bool, bool> mungeFunc)
    {
        this.Parameters[resultName] = mungeFunc(!string.IsNullOrEmpty(x));
    }

    private void OptionSetParseString(string x, string resultName)
    {
        this.Parameters[resultName] = x;
    }

    private bool ParseParameters(out IEnumerable<CommandResponse> errorResponses)
    {
        try
        {
            var remainder = this.OptionSet.Parse(this.Arguments);
            this.Arguments.Clear();
            foreach (var r in remainder)
            {
                this.Arguments.Add(r);
            }

            errorResponses = Array.Empty<CommandResponse>();
            return true;
        }
        catch (OptionException ex)
        {
            errorResponses = new[] { new CommandResponse { Message = ex.Message } };
            return false;
        }
    }

    /// <inheritdoc />
    public IEnumerable<CommandResponse> HelpMessage(string helpKey = null)
    {
        var helpMessages = this.Help();

        var methodInfos = this.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic);

        foreach (var info in methodInfos)
        {
            if (info.IsAbstract || info.IsConstructor || info.IsPrivate)
            {
                continue;
            }

            var invokAttr = info.GetAttribute<SubcommandInvocationAttribute>();
            if (invokAttr == null && info.Name != "Execute")
            {
                continue;
            }

            var helpAttr = info.GetAttribute<HelpAttribute>();
            if (helpAttr == null)
            {
                continue;
            }

            if (this.AllowedSubcommand(info))
            {
                var cmdName = invokAttr == null ? string.Empty : invokAttr.CommandName;
                helpMessages.Add(cmdName, helpAttr.HelpMessage);
            }
        }

        var commandTrigger = this.configurationProvider.CommandPrefix;

        if (helpMessages == null)
        {
            return new List<CommandResponse>();
        }

        if (helpKey != null && helpMessages.ContainsKey(helpKey))
        {
            return helpMessages[helpKey].ToCommandResponses(commandTrigger, this.CommandName, helpKey);
        }

        var help = new List<CommandResponse>();
        foreach (var helpMessage in helpMessages)
        {
            help.AddRange(helpMessage.Value.ToCommandResponses(commandTrigger, this.CommandName, helpMessage.Key));
        }

        return help;
    }

    protected virtual IEnumerable<CommandResponse> OnAccessDenied()
    {
        var response = new CommandResponse
        {
            Destination = CommandResponseDestination.PrivateMessage,
            Message = "Access denied, sorry. :(",
            IgnoreRedirection = true
        };

        return new List<CommandResponse> { response };
    }

    protected virtual IEnumerable<CommandResponse> OnMissingSubcommand()
    {
        var response = new CommandResponse
        {
            Destination = CommandResponseDestination.PrivateMessage,
            Message = "Unable to find the sub-command you requested, or a default handler for this command.",
            IgnoreRedirection = true
        };

        return new List<CommandResponse> { response };
    }

    protected virtual IEnumerable<CommandResponse> OnCompleted()
    {
        return null;
    }

    protected virtual IDictionary<string, HelpMessage> Help()
    {
        return new Dictionary<string, HelpMessage>();
    }
}