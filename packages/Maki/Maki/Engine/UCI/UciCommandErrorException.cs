//-----------------------------------------------------------------------
// <copyright file="UciCommandErrorException.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Engine.UCI
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// An error thrown when a specific UCI command did not execute correctly or was invalid.
    /// </summary>
    [Serializable]
    public class UciCommandErrorException : UciEngineException
    {
        private const string DefaultErrorMessage = "An error occurred executing a UCI command.";

        /// <summary>
        /// Initializes a new instance of the <see cref="UciCommandErrorException"/> class with the default error message.
        /// </summary>
        public UciCommandErrorException()
            : base(DefaultErrorMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UciCommandErrorException"/> class.
        /// </summary>
        /// <param name="message">The custom error message.</param>
        public UciCommandErrorException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UciCommandErrorException"/> class with the default error message.
        /// </summary>
        /// <param name="message">The custom error message.</param>
        /// <param name="inner">The inner exception.</param>
        public UciCommandErrorException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UciCommandErrorException"/> class with a streaming context.
        /// </summary>
        /// <param name="info">The serialization information.</param>
        /// <param name="context">The streaming context.</param>
        protected UciCommandErrorException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}