//-----------------------------------------------------------------------
// <copyright file="WebSocketManagerMiddleware.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.IO
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The request handler for the middleware application.
    /// </summary>
    public class WebSocketManagerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<WebSocketManagerMiddleware> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketManagerMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next step of the request chain.</param>
        /// <param name="webSocketHandler">The handler when a client websocket connects.</param>
        /// <param name="logger">The logger.</param>
        public WebSocketManagerMiddleware(
            RequestDelegate next,
            WebSocketHandler webSocketHandler,
            ILogger<WebSocketManagerMiddleware> logger)
        {
            this.next = next;
            this.WebSocketHandler = webSocketHandler;
            this.logger = logger;
        }

        private WebSocketHandler WebSocketHandler { get; set; }

        /// <summary>
        /// Invokes the middleware to handle an incoming HTTP request.
        /// </summary>
        /// <param name="context">The context of the request.</param>
        /// <returns>An awaitable task.</returns>
        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                return;
            }

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            await this.WebSocketHandler.OnConnected(socket);
            await this.Receive(socket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    await this.WebSocketHandler.ReceiveAsync(socket, result, buffer);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await this.WebSocketHandler.OnDisconnected(socket);
                }
            });
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(
                    buffer: new ArraySegment<byte>(buffer),
                    cancellationToken: CancellationToken.None);

                handleMessage(result, buffer);
            }
        }
    }
}