//-----------------------------------------------------------------------
// <copyright file="BestMoveReceivedEventArgs.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Client
{
    using Maki.Model;

    /// <summary>
    /// A struct for the data around a BestMoveReceived event.
    /// </summary>
    public readonly struct BestMoveReceivedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BestMoveReceivedEventArgs"/> struct.
        /// </summary>
        /// <param name="result">The parsed evaluated line.</param>
        public BestMoveReceivedEventArgs(EvaluatePositionResult result)
        {
            this.Result = result;
        }

        /// <summary>
        /// Gets the result of the evaluation.
        /// </summary>
        public EvaluatePositionResult Result { get; }
    }
}