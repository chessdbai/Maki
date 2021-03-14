//-----------------------------------------------------------------------
// <copyright file="PortOracle.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Embedded
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An inner class to determine what port the application is listening on.
    /// </summary>
    public class PortOracle
    {
        /// <summary>
        /// Gets or sets the port we're listening on.
        /// </summary>
        public List<Uri> Uris { get; set; }
    }
}