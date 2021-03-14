//-----------------------------------------------------------------------
// <copyright file="ListSupportedOptionsResponse.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Model
{
    using System.Collections.Generic;
    using Maki.Model.Options;

    /// <summary>
    /// The response from the ListSupportedOptions API call.
    /// </summary>
    public class ListSupportedOptionsResponse
    {
        /// <summary>
        /// Gets or sets the list of options the engine supports.
        /// </summary>
        public List<EngineOption> Options { get; set; }
    }
}