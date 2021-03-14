//-----------------------------------------------------------------------
// <copyright file="UciEngineException.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Engine.UCI
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// An abstract UCI exception that forms the base of the other custom UCI exceptions.
    /// </summary>
    [Serializable]
    public abstract class UciEngineException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UciEngineException"/> class with the default error message.
        /// </summary>
        public UciEngineException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UciEngineException"/> class.
        /// </summary>
        /// <param name="message">The custom error message.</param>
        public UciEngineException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UciEngineException"/> class with the default error message.
        /// </summary>
        /// <param name="message">The custom error message.</param>
        /// <param name="inner">The inner exception.</param>
        public UciEngineException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UciEngineException"/> class with a streaming context.
        /// </summary>
        /// <param name="info">The serialization information.</param>
        /// <param name="context">The streaming context.</param>
        protected UciEngineException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}