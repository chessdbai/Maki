//-----------------------------------------------------------------------
// <copyright file="StartEvaluationRequest.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Maki.Model.Validation;
    using ProtoBuf;

    /// <summary>
    /// An API request to start the evaluation from the current position.
    /// </summary>
    [ProtoContract]
    public class StartEvaluationRequest
    {
        /// <summary>
        /// Gets or sets the request id.
        /// </summary>
        [Required]
        [ProtoMember(1)]
        public string RequestId { get; set; }

        /// <summary>
        /// Gets or sets the position to start evaluating from.
        /// </summary>
        [Required]
        [ValidatedStartingPosition]
        [ProtoMember(2)]
        public StartingPosition Position { get; set; }

        /// <summary>
        /// Gets or sets the criteria the engine uses to know when
        /// it should stop the evaluation.
        /// </summary>
        [ProtoMember(3)]
        public CompletionCriteria CompletionCriteria { get; set; }

        /// <summary>
        /// Gets or sets a list of moves the engine is allowed to consider
        /// in the start position. If the list is null or empty, the engine
        /// will consider all moves.
        /// </summary>
        [ProtoMember(4)]
        public List<MoveFilter> SearchMoves { get; set; }

        /// <summary>
        /// Gets or sets the time control and remaining time for the players.
        /// </summary>
        [ProtoMember(5)]
        public ClockState Clock { get; set; }

        /// <summary>
        /// Gets or sets the list of engine options.
        /// </summary>
        [ProtoMember(6)]
        public Dictionary<string, OptionValue> EngineOptions { get; set; }
    }
}