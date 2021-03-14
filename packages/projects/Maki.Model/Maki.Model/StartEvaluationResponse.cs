//-----------------------------------------------------------------------
// <copyright file="StartEvaluationResponse.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Model
{
    using System.Collections.Generic;
    using ProtoBuf;

    /// <summary>
    /// A class for the response from the StartEvaluation API call.
    /// </summary>
    [ProtoContract]
    public class StartEvaluationResponse
    {
        /// <summary>
        /// Gets or sets the evaluated lines.
        /// </summary>
        public Dictionary<int, EvaluatedLine> Lines { get; set; }
    }
}