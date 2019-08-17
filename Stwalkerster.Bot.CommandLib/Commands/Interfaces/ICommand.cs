namespace Stwalkerster.Bot.CommandLib.Commands.Interfaces
{
    using System.Collections.Generic;
    using System.Threading;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    /// <summary>
    /// The Command interface.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets the command Completed Semaphore.
        /// <para>
        /// This semaphore can be set with <code>CommandCompletedSemaphore.WaitOne()</code> to suspend destruction of the command for a maximum of thirty seconds.
        /// </para>
        /// </summary>
        Semaphore CommandCompletedSemaphore { get; }

        bool Executed { get; }
        CommandExecutionStatus ExecutionStatus { get; }

        /// <summary>
        /// Returns the collection of arguments passed to this command
        /// </summary>
        IList<string> Arguments { get; }

        /// <summary>
        /// Returns the name under which this command was invoked
        /// </summary>
        string InvokedAs { get; }

        /// <summary>
        /// Returns the canonical name of this command
        /// </summary>
        string CommandName { get; }

        /// <inheritdoc />
        string CommandSource { get; }

        /// <summary>
        /// Gets the original string of data which was passed to this command as an argument.
        /// </summary>
        string OriginalArguments { get; }

        /// <inheritdoc />
        IEnumerable<string> RedirectionTarget { get; }

        /// <inheritdoc />
        IUser User { get; }

        IIrcClient Client { get; }

        /// <summary>
        /// Returns the subcommand which was invoked
        /// </summary>
        string SubCommand { get; }

        IEnumerable<CommandResponse> HelpMessage(string helpKey = null);

        /// <summary>
        /// Runs the command
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{CommandResponse}"/>.
        /// </returns>
        IEnumerable<CommandResponse> Run();
    }
}