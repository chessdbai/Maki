//-----------------------------------------------------------------------
// <copyright file="WebSocketHandler.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.IO
{
    using System;
    using System.Collections.Generic;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A base class for subclasses to use for handling websocket requests.
    /// </summary>
    public abstract class WebSocketHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketHandler"/> class.
        /// </summary>
        /// <param name="webSocketConnectionManager">The connection manager instance..</param>
        public WebSocketHandler(WebSocketConnectionManager webSocketConnectionManager)
        {
            this.WebSocketConnectionManager = webSocketConnectionManager;
        }

        /// <summary>
        /// Gets or sets the connection manager.
        /// </summary>
        protected WebSocketConnectionManager WebSocketConnectionManager { get; set; }

        /// <summary>
        /// Action to take when a websocket client connects.
        /// </summary>
        /// <param name="socket">The connecting websocket.</param>
        /// <returns>An awaitable task.</returns>
        #pragma warning disable 1998
        public virtual async Task OnConnected(WebSocket socket)
        {
            this.WebSocketConnectionManager.AddSocket(socket);
        }
        #pragma warning restore 1998

        /// <summary>
        /// Action to take when a websocket client disconnects.
        /// </summary>
        /// <param name="socket">The disconnecting websocket.</param>
        /// <returns>An awaitable task.</returns>
        public virtual async Task OnDisconnected(WebSocket socket)
        {
            await this.WebSocketConnectionManager.RemoveSocket(this.WebSocketConnectionManager.GetId(socket));
        }

        /// <summary>
        /// Sends a message to a particular websocket client.
        /// </summary>
        /// <param name="socket">The websocket client.</param>
        /// <param name="message">The message to send.</param>
        /// <returns>An awaitable task.</returns>
        public async Task SendMessageAsync(WebSocket socket, string message)
        {
            if (socket.State != WebSocketState.Open)
            {
                return;
            }

            await socket.SendAsync(
                buffer: new ArraySegment<byte>(
                    array: Encoding.ASCII.GetBytes(message),
                    offset: 0,
                    count: message.Length),
                messageType: WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: CancellationToken.None);
        }

        /// <summary>
        /// Sends a message to a single websocket by id.
        /// </summary>
        /// <param name="socketId">The websocket ID to send the message to.</param>
        /// <param name="message">The message to send.</param>
        /// <returns>An awaitable task.</returns>
        public async Task SendMessageAsync(string socketId, string message)
        {
            await this.SendMessageAsync(this.WebSocketConnectionManager.GetSocketById(socketId), message);
        }

        /// <summary>
        /// Sends a message to all connected websockets.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>An awaitable task.</returns>
        public async Task SendMessageToAllAsync(string message)
        {
            foreach (var pair in this.WebSocketConnectionManager.GetAll())
            {
                if (pair.Value.State == WebSocketState.Open)
                {
                    await this.SendMessageAsync(pair.Value, message);
                }
            }
        }

        /// <summary>
        /// The action to take when a client message is received.
        /// </summary>
        /// <param name="socket">The websocket client sending the message.</param>
        /// <param name="result">The receive result.</param>
        /// <param name="buffer">The byte buffer containing the data.</param>
        /// <returns>An awaitable task.</returns>
        public abstract Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);
    }
}