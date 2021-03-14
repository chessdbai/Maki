//-----------------------------------------------------------------------
// <copyright file="EmbeddedExtensions.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Embedded
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Static class with extension methods for connecting to the embedded Maki server.
    /// </summary>
    public static class EmbeddedExtensions
    {
        /// <summary>
        /// Gets the URIs that the host is listening on.
        /// Since we're dynamically choosing a port, this is required to
        /// be able to create a client that can connect.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns>A list of URIs.</returns>
        public static List<Uri> GetListeningUris(this IHost host)
        {
            var portOracle = host.Services.GetRequiredService<PortOracle>();
            return portOracle.Uris;
        }
    }
}