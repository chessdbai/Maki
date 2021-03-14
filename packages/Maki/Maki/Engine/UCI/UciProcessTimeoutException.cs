//-----------------------------------------------------------------------
// <copyright file="UciProcessTimeoutException.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Engine.UCI
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// An exception thrown when the engine did not respond to the UCI command in a timely manner.
    /// </summary>
    [Serializable]
    public class UciProcessTimeoutException : UciEngineException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UciProcessTimeoutException"/> class with the default error message.
        /// </summary>
        public UciProcessTimeoutException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UciProcessTimeoutException"/> class.
        /// </summary>
        /// <param name="message">The custom error message.</param>
        public UciProcessTimeoutException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UciProcessTimeoutException"/> class with the default error message.
        /// </summary>
        /// <param name="message">The custom error message.</param>
        /// <param name="inner">The inner exception.</param>
        public UciProcessTimeoutException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UciProcessTimeoutException"/> class with a streaming context.
        /// </summary>
        /// <param name="info">The serialization information.</param>
        /// <param name="context">The streaming context.</param>
        protected UciProcessTimeoutException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}