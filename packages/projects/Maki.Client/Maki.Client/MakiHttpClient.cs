//-----------------------------------------------------------------------
// <copyright file="MakiHttpClient.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Maki.Model;
    using Maki.Model.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// A client for interacting with the Maki UCI engine wrapper server
    /// over the websocket protocol.
    /// </summary>
    public class MakiHttpClient : IMakiClient, IAsyncDisposable
    {
        #pragma warning disable SA1306
        private static readonly TimeSpan MaxWaitTime = TimeSpan.FromSeconds(15);
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings()
        {
            Converters = new List<JsonConverter>(new[]
            {
                new TimeSpanConverter(),
            }),
        };
        #pragma warning restore SA1306

        private readonly Uri wsUrl;
        private readonly HttpClient httpClient;
        private readonly ClientWebSocket wsClient;
        private readonly DataFormat dataFormat;
        private readonly CancellationTokenSource cancellationTokenSource;
        private volatile bool runMessageLoopThread = false;
        private Thread messageLoopThread;
        private CancellationToken? cancellationToken = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="MakiHttpClient" /> class.
        /// </summary>
        /// <param name="options">The configuration options to set the client up.</param>
        public MakiHttpClient(MakiHttpClientOptions options)
        {
            if (options.WebsocketUri == null)
            {
                this.wsUrl = ConstructWebsocketUriFromHttpUri(options.HttpUri.ToString(), "ws");
            }
            else
            {
                this.wsUrl = options.WebsocketUri;
            }

            if (options.HttpHandler != null)
            {
                this.httpClient = new HttpClient(options.HttpHandler);
            }
            else
            {
                this.httpClient = new HttpClient();
            }

            this.httpClient.BaseAddress = options.HttpUri;

            if (options.Format != null)
            {
                this.dataFormat = options.Format!.Value;
            }
            else
            {
                // A reasonable default.
                this.dataFormat = DataFormat.Json;
            }

            this.wsClient = new ClientWebSocket();
            this.cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// An event fired when the engine has sent an evaluation line.
        /// </summary>
        public event EventHandler<LineReceivedEventArgs> ReceivedLine;

        /// <summary>
        /// An event fired when the best move is received.
        /// </summary>
        public event EventHandler<BestMoveReceivedEventArgs> ReceivedBestMove;

        /// <summary>
        /// An event fired when the best move is received.
        /// </summary>
        public event EventHandler<BestLinesReceivedEventArgs> ReceivedBestLines;

        /// <inheritdoc cref="IMakiClient" />
        public async Task SubscribeToEngineStreamAsync() => await this.SubscribeToEngineStreamAsync(CancellationToken.None);

        /// <inheritdoc cref="IMakiClient" />
        public async Task SubscribeToEngineStreamAsync(CancellationToken cancellationToken)
        {
            await this.wsClient.ConnectAsync(this.wsUrl, cancellationToken);
            this.runMessageLoopThread = true;
            this.messageLoopThread = new Thread(this.RunMessageLoop);
            this.cancellationToken = this.cancellationTokenSource.Token;
            this.messageLoopThread.Start();
        }

        /// <inheritdoc cref="IMakiClient" />
        public async Task UnsubscribeFromEngineStreamAsync() => await this.SubscribeToEngineStreamAsync(CancellationToken.None);

        /// <inheritdoc cref="IMakiClient" />
        public async Task UnsubscribeFromEngineStreamAsync(CancellationToken cancellationToken)
        {
            await this.wsClient.ConnectAsync(this.wsUrl, cancellationToken);
            this.runMessageLoopThread = true;
            this.messageLoopThread = new Thread(this.RunMessageLoop);
            this.cancellationToken = this.cancellationTokenSource.Token;
            this.messageLoopThread.Start();
        }

        /// <inheritdoc cref="IMakiClient" />
        public async Task<GetEngineIdResponse> GetEngineIdAsync() => await this.GetEngineIdAsync(CancellationToken.None);

        /// <inheritdoc cref="IMakiClient" />
        public async Task<GetEngineIdResponse> GetEngineIdAsync(CancellationToken cancellationToken)
        {
            using var httpRequest = this.CreateGetRequest("/id");
            using var response = await this.httpClient.SendAsync(httpRequest, cancellationToken);
            return await this.ReadHttpResponseAsync<GetEngineIdResponse>(response);
        }

        /// <inheritdoc cref="IMakiClient" />
        public async Task SetEngineDebugFlagAsync(SetEngineDebugFlagRequest request) =>
            await this.SetEngineDebugFlagAsync(request, CancellationToken.None);

        /// <inheritdoc cref="IMakiClient" />
        public async Task SetEngineDebugFlagAsync(SetEngineDebugFlagRequest request, CancellationToken cancellationToken)
        {
            using var httpRequest = this.CreatePostRequest(request, "/debug");
            using var httpResponse = await this.httpClient.SendAsync(httpRequest, cancellationToken);
            httpResponse.EnsureSuccessStatusCode();
        }

        /// <inheritdoc cref="IMakiClient" />
        public async Task<ListSupportedOptionsResponse> ListSupportedOptionsAsync() =>
            await this.ListSupportedOptionsAsync(CancellationToken.None);

        /// <inheritdoc cref="IMakiClient" />
        public async Task<ListSupportedOptionsResponse> ListSupportedOptionsAsync(CancellationToken cancellationToken)
        {
            using var httpRequest = this.CreateGetRequest("/options");
            using var response = await this.httpClient.SendAsync(httpRequest, cancellationToken);
            return await this.ReadHttpResponseAsync<ListSupportedOptionsResponse>(response);
        }

        /// <inheritdoc cref="IMakiClient" />
        public async Task<StartEvaluationResponse> StartEvaluationAsync(StartEvaluationRequest request) => await this.StartEvaluationAsync(request, CancellationToken.None);

        /// <inheritdoc cref="IMakiClient" />
        public async Task<StartEvaluationResponse> StartEvaluationAsync(StartEvaluationRequest request, CancellationToken cancellationToken)
        {
            using var httpRequest = this.CreatePostRequest<StartEvaluationRequest>(request, "evaluation");
            using var httpResponse = await this.httpClient.SendAsync(httpRequest, cancellationToken);
            return await this.ReadHttpResponseAsync<StartEvaluationResponse>(httpResponse);
        }

        /// <inheritdoc cref="IMakiClient" />
        public async Task<string> HealthCheckAsync() => await this.HealthCheckAsync(CancellationToken.None);

        /// <inheritdoc cref="IMakiClient" />
        public async Task<string> HealthCheckAsync(CancellationToken cancellationToken)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, "health");
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            using var response = await this.httpClient.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Cleans up resources in an asynchronous fashion.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        public async ValueTask DisposeAsync()
        {
            this.runMessageLoopThread = false;
            this.cancellationTokenSource.Cancel();

            var startTime = DateTime.UtcNow;
            TimeSpan timePassed = TimeSpan.Zero;
            if (this.messageLoopThread != null && this.messageLoopThread.IsAlive)
            {
                while (this.messageLoopThread.IsAlive && timePassed < MaxWaitTime)
                {
                    await Task.Delay(100);
                    timePassed = DateTime.UtcNow - startTime;
                }
            }

            if (this.wsClient != null && this.wsClient.State == WebSocketState.Open)
            {
                await this.wsClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "AsyncDisposal", CancellationToken.None);
            }

            this.httpClient.Dispose();
            this.cancellationTokenSource.Dispose();
        }

        /// <summary>
        /// Creates the websocket URI from the HTTP uri.
        /// </summary>
        /// <param name="httpUri">The HTTP URI.</param>
        /// <param name="wsResource">The path of the websocket resource.</param>
        /// <returns>The websocket URI.</returns>
        /// <exception cref="ArgumentException">On an invalid URI, such as one that doesn't start with http or https.</exception>
        internal static Uri ConstructWebsocketUriFromHttpUri(string httpUri, string wsResource)
        {
            string wsUri;
            if (httpUri.StartsWith("http://"))
            {
                wsUri = httpUri.Replace("http://", "ws://");
            }
            else if (httpUri.StartsWith("https://"))
            {
                wsUri = httpUri.Replace("https://", "wss://");
            }
            else
            {
                throw new ArgumentException($"Unknown protocol in HTTP URI '{httpUri}'.");
            }

            wsUri = wsUri.TrimEnd('/');
            if (!string.IsNullOrEmpty(wsResource))
            {
                string resource = wsResource.TrimStart('/');
                wsUri = wsUri + "/" + resource;
            }

            return new Uri(wsUri, UriKind.Absolute);
        }

        private void RunMessageLoop()
        {
            while (this.runMessageLoopThread && this.wsClient.State == WebSocketState.Open)
            {
                try
                {
                    Console.WriteLine($"Running message loop.");
                    var msg = this.ReceiveMessagesAsync(this.wsClient).Result;
                    if (msg != null)
                    {
                        var segment = JsonConvert.DeserializeObject<EngineOutputSegment>(msg);
                        if (segment.Line != null)
                        {
                            this.ReceivedLine?.Invoke(this, new LineReceivedEventArgs(segment.Line));
                        }
                        else if (segment.Result != null)
                        {
                            this.ReceivedBestMove?.Invoke(this, new BestMoveReceivedEventArgs(segment.Result));
                        }
                        else if (segment.BestLines != null)
                        {
                            this.ReceivedBestLines?.Invoke(this, new BestLinesReceivedEventArgs(segment.BestLines.ToImmutableDictionary()));
                        }
                    }
                }
                catch (AggregateException e)
                {
                    if (e.InnerExceptions.Count > 0 && e.InnerExceptions.First() is TaskCanceledException)
                    {
                        break;
                    }
                }
            }
        }

        private async Task<string> ReceiveMessagesAsync(ClientWebSocket socket)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[8192]);

            WebSocketReceiveResult result = null;

            await using var ms = new MemoryStream();
            do
            {
                result = await socket.ReceiveAsync(buffer, this.cancellationToken!.Value!);
                ms.Write(buffer.Array!, buffer.Offset, result.Count);
            }
            while (!result.EndOfMessage);

            ms.Seek(0, SeekOrigin.Begin);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                using var reader = new StreamReader(ms, Encoding.UTF8);
                return await reader.ReadToEndAsync();
            }

            return null;
        }

        private HttpRequestMessage CreateGetRequest(string resource)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, resource);
            switch (this.dataFormat)
            {
                case DataFormat.Json:
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    break;
                case DataFormat.ProtocolBuffers:
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-protobuf"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown data format '{this.dataFormat}'.");
            }

            return request;
        }

        private HttpRequestMessage CreatePostRequest<TReq>(TReq req, string resource)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, resource);
            switch (this.dataFormat)
            {
                case DataFormat.Json:
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    request.Content = this.CreateJsonContent(req);
                    break;
                case DataFormat.ProtocolBuffers:
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-protobuf"));
                    request.Content = this.CreateProtobufContent(req);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown data format '{this.dataFormat}'.");
            }

            return request;
        }

        private async Task<TRes> ReadHttpResponseAsync<TRes>(HttpResponseMessage message)
        {
            message.EnsureSuccessStatusCode();
            string contentType = message.Content.Headers.ContentType.MediaType;
            switch (contentType)
            {
                case "application/json":
                    return await this.ReadJsonResponseAsync<TRes>(message.Content);
                case "application/x-protobuf":
                    return await this.ReadProtobufResponseAsync<TRes>(message.Content);
                default:
                    throw new ArgumentException($"Unknown response content type: '{contentType}'.");
            }
        }

        private HttpContent CreateJsonContent<TReq>(TReq req)
        {
            string json = JsonConvert.SerializeObject(req, SerializerSettings);
            var content = new StringContent(json);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            return content;
        }

        private HttpContent CreateProtobufContent<TReq>(TReq req)
        {
            var ms = new MemoryStream();
            ProtoBuf.Serializer.Serialize(ms, req);
            ms.Seek(0, SeekOrigin.Begin);
            var content = new StreamContent(ms);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-protobuf");
            return content;
        }

        private async Task<TRes> ReadJsonResponseAsync<TRes>(HttpContent content)
        {
            using var responseStream = await content.ReadAsStreamAsync();
            using var reader = new StreamReader(responseStream);
            string json = await reader.ReadToEndAsync();
            return JsonConvert.DeserializeObject<TRes>(json, SerializerSettings);
        }

        private async Task<TRes> ReadProtobufResponseAsync<TRes>(HttpContent content)
        {
            using var responseStream = await content.ReadAsStreamAsync();
            return ProtoBuf.Serializer.Deserialize<TRes>(responseStream);
        }
    }
}