//-----------------------------------------------------------------------
// <copyright file="DataFormat.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Client
{
    /// <summary>
    /// The Maki server supports several serialization formats.
    /// </summary>
    public enum DataFormat
    {
        /// <summary>
        /// Use standard JSON for serialization and deserialization.
        /// </summary>
        Json,

        /// <summary>
        /// Use protocol buffers for serialization and deserialization.
        /// </summary>
        ProtocolBuffers,
    }
}