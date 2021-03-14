//-----------------------------------------------------------------------
// <copyright file="MakiHttpClientOptions.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Client
{
    using System;
    using System.Net.Http;

    /// <summary>
    /// A set of options for configuring the communication mechanisms between the
    /// Maki client and server.
    /// </summary>
    public class MakiHttpClientOptions
    {
        /// <summary>
        /// Gets or sets the data format used for transferring data to and from
        /// the Maki server.
        /// </summary>
        public DataFormat? Format { get; set; }

        /// <summary>
        /// Gets or sets the HTTP URI used for RESTful API calls.
        /// </summary>
        public Uri HttpUri { get; set; }

        /// <summary>
        /// Gets or sets the URI to use for websocket communications when engine streaming
        /// is enabled.
        /// </summary>
        public Uri WebsocketUri { get; set; }

        /// <summary>
        /// Gets or sets a custom HTTP handler to use when making requests.
        /// This only applies to the REST API calls.
        /// </summary>
        public HttpClientHandler HttpHandler { get; set; }
    }
}