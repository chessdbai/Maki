//-----------------------------------------------------------------------
// <copyright file="BestLinesReceivedEventArgs.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Client
{
    using System.Collections.Immutable;
    using Maki.Model;

    /// <summary>
    ///  A struct to store data around the BestLinesReceived event.
    /// </summary>
    public readonly struct BestLinesReceivedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BestLinesReceivedEventArgs"/> struct.
        /// </summary>
        /// <param name="bestLines">The best lines dictionary.</param>
        public BestLinesReceivedEventArgs(IImmutableDictionary<int, EvaluatedLine> bestLines)
        {
            this.BestLines = bestLines;
        }

        /// <summary>
        /// Gets the dictionary of best lines.
        /// </summary>
        public IImmutableDictionary<int, EvaluatedLine> BestLines { get; }
    }
}