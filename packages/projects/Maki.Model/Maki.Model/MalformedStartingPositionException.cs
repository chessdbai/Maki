//-----------------------------------------------------------------------
// <copyright file="MalformedStartingPositionException.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Model
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// An error indicating that evaluation failed because the provided starting
    /// position was not valid, or was missing.
    /// </summary>
    [Serializable]
    public class MalformedStartingPositionException : Exception
    {
        private const string DefaultMessage = "The provided start position was invalid or missing.";

        /// <summary>
        /// Initializes a new instance of the <see cref="MalformedStartingPositionException"/> class
        /// with the default message.
        /// </summary>
        public MalformedStartingPositionException()
            : base(DefaultMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MalformedStartingPositionException"/> class.
        /// </summary>
        /// <param name="message">The error message to use.</param>
        public MalformedStartingPositionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MalformedStartingPositionException"/> class.
        /// </summary>
        /// <param name="message">The error message to use.</param>
        /// <param name="inner">The inner exception to use.</param>
        public MalformedStartingPositionException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MalformedStartingPositionException"/> class.
        /// </summary>
        /// <param name="info">The serialization information.</param>
        /// <param name="context">The streaming context.</param>
        protected MalformedStartingPositionException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}