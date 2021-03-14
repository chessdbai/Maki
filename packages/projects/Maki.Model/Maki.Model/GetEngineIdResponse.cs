//-----------------------------------------------------------------------
// <copyright file="GetEngineIdResponse.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Model
{
    /// <summary>
    /// A response to the GetEngineId API call.
    /// </summary>
    public class GetEngineIdResponse
    {
        /// <summary>
        /// Gets or sets the engine name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the engine Author.
        /// </summary>
        public string Author { get; set; }
    }
}