//-----------------------------------------------------------------------
// <copyright file="LineReceivedEventArgs.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Client
{
    using Maki.Model;

    /// <summary>
    /// Data associated with a 'LineReceived' event in a <see cref="IMakiClient"> MakiClient</see>.
    /// </summary>
    public readonly struct LineReceivedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineReceivedEventArgs"/> struct.
        /// </summary>
        /// <param name="line">The parsed evaluated line.</param>
        public LineReceivedEventArgs(EvaluatedLine line)
        {
            this.Line = line;
        }

        /// <summary>
        /// Gets the parsed evaluated line.
        /// </summary>
        public EvaluatedLine Line { get; }
    }
}