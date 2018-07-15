namespace Stwalkerster.Bot.CommandLib.Commands.Interfaces
{
    using System.Collections.Generic;
    using System.Threading;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.IrcClient.Model.Interfaces;

    /// <summary>
    /// The Command interface.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets the source (where the command was triggered).
        /// </summary>
        string CommandSource { get; }

        /// <summary>
        /// Gets or sets the redirection target.
        /// </summary>
        IEnumerable<string> RedirectionTarget { get; }
        
        /// <summary>
        /// Gets the user who triggered the command.
        /// </summary>
        IUser User { get; }

        /// <summary>
        /// Gets the command Completed Semaphore.
        /// <para>
        /// This semaphore can be set with <code>CommandCompletedSemaphore.WaitOne()</code> to suspend destruction of the command for a maximum of thirty seconds.
        /// </para>
        /// </summary>
        Semaphore CommandCompletedSemaphore { get; }

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