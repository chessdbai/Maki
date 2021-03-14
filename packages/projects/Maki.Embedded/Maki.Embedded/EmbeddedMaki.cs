//-----------------------------------------------------------------------
// <copyright file="EmbeddedMaki.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Embedded
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Maki.Client;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// An embedded maki server that sets up the asp.net core
    /// environment to allow for multiple instances, a la DynamoDBEmbedded.
    /// </summary>
    public class EmbeddedMaki : IDisposable
    {
        private readonly IHost host;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedMaki"/> class.
        /// </summary>
        /// <param name="host">The embedded maki host.</param>
        public EmbeddedMaki(IHost host)
        {
            this.host = host;
        }

        /// <summary>
        /// Starts up the embedded Maki host.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        public async Task StartAsync()
        {
            await this.host.StartAsync();
        }

        /// <summary>
        /// Stops the embedded Maki host.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        public async Task StopAsync() => await this.host.StopAsync();

        /// <summary>
        /// Create a MakiClient that is connected to this embedded instance.
        /// </summary>
        /// <returns>The Maki client.</returns>
        public IMakiClient CreateClient()
        {
            var uris = this.host.GetListeningUris();
            return new MakiHttpClient(new MakiHttpClientOptions()
            {
                HttpUri = uris.First(),
            });
        }

        /// <inheritdoc cref="IDisposable" />
        public void Dispose() => this.host?.Dispose();
    }
}