//-----------------------------------------------------------------------
// <copyright file="CompletionCriteria.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Model
{
    using System;
    using ProtoBuf;

    /// <summary>
    /// A class with data that tells the engine when it has reached
    /// the end of the requested evaluation session.
    /// </summary>
    [ProtoContract]
    public class CompletionCriteria
    {
        /// <summary>
        /// Gets or sets the max time to calculate for.
        /// </summary>
        [ProtoMember(1)]
        public TimeSpan? PonderTime { get; set; }

        /// <summary>
        /// Gets or sets the max depth.
        /// </summary>
        [ProtoMember(2)]
        public ushort? MaxDepth { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of moves to look for mate.
        /// </summary>
        [ProtoMember(3)]
        public ushort? Mate { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of nodes to search.
        /// </summary>
        [ProtoMember(4)]
        public ulong? MaxNodes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the analysis should be infinite.
        /// </summary>
        [ProtoMember(5)]
        public bool Infinite { get; set; }
    }
}