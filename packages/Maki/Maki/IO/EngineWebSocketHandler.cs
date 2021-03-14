//-----------------------------------------------------------------------
// <copyright file="EngineWebSocketHandler.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.IO
{
    using System;
    using System.Collections.Generic;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading.Tasks;
    using Maki.Engine;
    using Maki.Model;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// The handler class to manage a single incoming websocket client.
    /// </summary>
    public class EngineWebSocketHandler : WebSocketHandler
    {
        private readonly ILogger<EngineWebSocketHandler> logger;
        private readonly EngineBridge engineBridge;
        private readonly Dictionary<string, StringBuilder> socketBuffer = new Dictionary<string, StringBuilder>();
        private readonly Dictionary<int, EvaluatedLine> bestLines = new Dictionary<int, EvaluatedLine>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineWebSocketHandler"/> class.
        /// </summary>
        /// <param name="webSocketConnectionManager">The connection manager that will track the lifetime of the websocket.</param>
        /// <param name="engineBridge">The engine bridge.</param>
        /// <param name="logger">The logger.</param>
        public EngineWebSocketHandler(
            WebSocketConnectionManager webSocketConnectionManager,
            EngineBridge engineBridge,
            ILogger<EngineWebSocketHandler> logger)
            : base(webSocketConnectionManager)
        {
            this.logger = logger;
            this.engineBridge = engineBridge;
            this.engineBridge.ReceivedEvaluationUpdate += (sender, segment) =>
            {
                this.BroadcastSegmentAsync(segment).Wait();
            };
            this.engineBridge.ReceivedEngineError += (sender, str) => this.ErrorReceived(str);
        }

        /// <summary>
        /// Overrides the receive behavior to pipe input and output to the engine process.
        /// </summary>
        /// <param name="socket">The websocket client.</param>
        /// <param name="result">The result.</param>
        /// <param name="buffer">The buffer.</param>
        /// <returns>An awaitable task.</returns>
        #pragma warning disable 1998
        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            string id = this.WebSocketConnectionManager.GetId(socket);
            string msg = Encoding.UTF8.GetString(buffer);
            if (!result.EndOfMessage)
            {
                this.logger.LogInformation($"{id}: PartialMessage - '{msg}'");
            }

            if (!this.socketBuffer.ContainsKey(id))
            {
                this.socketBuffer.Add(id, new StringBuilder());
            }

            lock (this.socketBuffer)
            {
                lock (this.socketBuffer[id])
                {
                    this.socketBuffer[id].Append(msg);
                    if (result.EndOfMessage)
                    {
                        string message = this.socketBuffer[id].ToString();
                        this.socketBuffer[id].Clear();

                        this.logger.LogInformation($"{id}: FullMessage - '{message}'");

                        // this.FullMessageReceivedAsync(socket, message).Wait();
                    }
                }
            }
        }
        #pragma warning restore 1998

        /// <summary>
        /// Overrides the disconnect behavior to update the <see cref="WebSocketConnectionManager" />.
        /// </summary>
        /// <param name="socket">The disconnected websocket.</param>
        /// <returns>An awaitable task.</returns>
        public override async Task OnDisconnected(WebSocket socket)
        {
            await this.WebSocketConnectionManager.RemoveSocket(this.WebSocketConnectionManager.GetId(socket));

            bool anyClientsLeft = this.WebSocketConnectionManager.GetAll().Keys.Count > 0;
            if (!anyClientsLeft)
            {
                // Environment.Exit(0);
            }
        }

        private void ErrorReceived(string errorText)
        {
            if (errorText == null)
            {
                return;
            }

            Console.Error.WriteLine(errorText);
            var segment = new EngineOutputSegment()
            {
                Error = errorText,
            };
            this.BroadcastSegmentAsync(segment).Wait();
        }

        private async Task BroadcastSegmentAsync(EngineOutputSegment segment)
        {
            if (segment == null)
            {
                return;
            }

            string message = JsonConvert.SerializeObject(segment);
            var removeSocketIds = new List<string>();
            var allClients = this.WebSocketConnectionManager?.GetAll();
            if (allClients == null)
            {
                return;
            }

            foreach (KeyValuePair<string, WebSocket> keyValuePair in allClients)
            {
                try
                {
                    KeyValuePair<string, WebSocket> pair = keyValuePair;
                    if (pair.Value.State == WebSocketState.Open)
                    {
                        await this.SendMessageAsync(pair.Value, message);
                    }
                    else
                    {
                        removeSocketIds.Add(pair.Key);
                    }
                }
                catch (Exception e)
                {
                    this.logger.LogWarning($"Failure to send segment to a particular websocket.", e);
                    removeSocketIds.Add(keyValuePair.Key);
                }
            }

            if (removeSocketIds.Count > 0)
            {
                this.logger.LogInformation($"Found {removeSocketIds.Count} dead sockets to clean up.");
                foreach (var socketId in removeSocketIds)
                {
                    try
                    {
                        this.WebSocketConnectionManager.RemoveSocket(socketId).Wait();
                    }
                    catch (Exception e)
                    {
                        this.logger.LogWarning($"Failed to remove socket with id {socketId}.", e);
                    }
                }
            }
        }
    }
}