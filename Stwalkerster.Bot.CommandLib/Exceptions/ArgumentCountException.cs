﻿namespace Stwalkerster.Bot.CommandLib.Exceptions
{
    using System;

    /// <summary>
    /// The argument count exception.
    /// </summary>
    [Serializable]
    public class ArgumentCountException : CommandExecutionException
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="ArgumentCountException"/> class.
        /// </summary>
        /// <param name="expectedCount">
        /// The expected count.
        /// </param>
        /// <param name="actualCount">
        /// The actual count.
        /// </param>
        /// <remarks>
        /// You probably want the other constructor
        /// </remarks>
        public ArgumentCountException(int expectedCount, int actualCount)
            : this(expectedCount, actualCount, null)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ArgumentCountException"/> class.
        /// </summary>
        /// <param name="expectedCount">
        /// The expected count.
        /// </param>
        /// <param name="actualCount">
        /// The actual count.
        /// </param>
        /// <param name="helpKey">
        /// The help Key.
        /// </param>
        public ArgumentCountException(int expectedCount, int actualCount, string helpKey)
            : base($"Insufficient arguments to command. Expected {expectedCount}, got {actualCount}.")
        {
            this.ExpectedCount = expectedCount;
            this.ActualCount = actualCount;
            this.HelpKey = helpKey;
        }

        public ArgumentCountException() : base("Insufficient arguments to command.")
        {
            this.HelpKey = null;
        }

        #endregion

        #region Public Properties

        public int ExpectedCount { get; }
        public int ActualCount { get; }

        /// <summary>
        /// Gets the help key.
        /// </summary>
        public string HelpKey { get; }

        #endregion
    }
}