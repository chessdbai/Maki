//-----------------------------------------------------------------------
// <copyright file="WebSocketConnectionManager.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.IO
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// A class to manage multiple client connections to the server.
    /// </summary>
    public class WebSocketConnectionManager
    {
        private readonly ILogger<WebSocketConnectionManager> logger;
        private ConcurrentDictionary<string, WebSocket> sockets = new ConcurrentDictionary<string, WebSocket>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketConnectionManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public WebSocketConnectionManager(ILogger<WebSocketConnectionManager> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Gets a websocket client by its ID.
        /// </summary>
        /// <param name="id">The ID of the websocket.</param>
        /// <returns>The websocket client.</returns>
        public WebSocket GetSocketById(string id)
        {
            return this.sockets.FirstOrDefault(p => p.Key == id).Value;
        }

        /// <summary>
        /// Gets all the known websocket clients along with their associated ID.
        /// </summary>
        /// <returns>An ID to client map.</returns>
        public ConcurrentDictionary<string, WebSocket> GetAll()
        {
            return this.sockets;
        }

        /// <summary>
        /// Gets the ID of a particular websocket.
        /// </summary>
        /// <param name="socket">The websocket.</param>
        /// <returns>The ID of the given websocket.</returns>
        public string GetId(WebSocket socket)
        {
            return this.sockets.FirstOrDefault(p => p.Value == socket).Key;
        }

        /// <summary>
        /// Adds a web socket client.
        /// </summary>
        /// <param name="socket">The websocket to add.</param>
        public void AddSocket(WebSocket socket)
        {
            this.sockets.TryAdd(this.CreateConnectionId(), socket);
        }

        /// <summary>
        /// Removes a websocket from the list.
        /// </summary>
        /// <param name="id">The ID of the websocket to remove.</param>
        /// <returns>An awaitable task.</returns>
        public async Task RemoveSocket(string id)
        {
            WebSocket socket;
            this.sockets.TryRemove(id, out socket);

            await socket.CloseAsync(
                closeStatus: WebSocketCloseStatus.NormalClosure,
                statusDescription: "Closed by the WebSocketManager",
                cancellationToken: CancellationToken.None);
        }

        private string CreateConnectionId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}