//-----------------------------------------------------------------------
// <copyright file="SetEngineDebugFlagRequest.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Model
{
    using ProtoBuf;

    /// <summary>
    /// An API request to start the evaluation from the current position.
    /// </summary>
    [ProtoContract]
    public class SetEngineDebugFlagRequest
    {
        /// <summary>
        /// Gets or sets a value indicating whether the engine should
        /// be set to debug mode.
        /// </summary>
        [ProtoMember(1)]
        public bool EnableDebugMode { get; set; }
    }
}