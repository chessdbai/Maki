//-----------------------------------------------------------------------
// <copyright file="IMakiClient.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Maki.Model;

    /// <summary>
    /// An interface specifying the required methods to be a Maki Client.
    /// </summary>
    public interface IMakiClient : IAsyncDisposable
    {
        /// <summary>
        /// An event that fires when a line is received from the engine.
        /// </summary>
        event EventHandler<LineReceivedEventArgs> ReceivedLine;

        /// <summary>
        /// An event fired when the best move is received.
        /// </summary>
        event EventHandler<BestMoveReceivedEventArgs> ReceivedBestMove;

        /// <summary>
        /// An event fired when the best move is received.
        /// </summary>
        event EventHandler<BestLinesReceivedEventArgs> ReceivedBestLines;

        /// <summary>
        /// Gets the engine identification information, such as engine name and author.
        /// </summary>
        /// <returns>The identification information from the engine.</returns>
        Task<GetEngineIdResponse> GetEngineIdAsync();

        /// <summary>
        /// Gets the engine identification information, such as engine name and author.
        /// </summary>
        /// <param name="cancellationToken">A custom cancellation token.</param>
        /// <returns>The identification information from the engine.</returns>
        Task<GetEngineIdResponse> GetEngineIdAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Instructs the engine to turn debug mode on or off.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>An awaitable task.</returns>
        Task SetEngineDebugFlagAsync(SetEngineDebugFlagRequest request);

        /// <summary>
        /// Instructs the engine to turn debug mode on or off.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">A custom cancellation token.</param>
        /// <returns>An awaitable task.</returns>
        Task SetEngineDebugFlagAsync(SetEngineDebugFlagRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Connect to the server.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        Task SubscribeToEngineStreamAsync();

        /// <summary>
        /// Connect to the server.
        /// </summary>
        /// <param name="cancellationToken">A custom cancellation token.</param>
        /// <returns>An awaitable task.</returns>
        Task SubscribeToEngineStreamAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Disconnect from the realtime streaming endpoint. This will
        /// have no effect on any ongoing evaluations.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        Task UnsubscribeFromEngineStreamAsync();

        /// <summary>
        /// Disconnect from the realtime streaming endpoint. This will
        /// have no effect on any ongoing evaluations.
        /// </summary>
        /// <param name="cancellationToken">A custom cancellation token.</param>
        /// <returns>An awaitable task.</returns>
        Task UnsubscribeFromEngineStreamAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Lists the configurable UCI options supported by the engine.
        /// </summary>
        /// <returns>A response object containing all the UCI options.</returns>
        Task<ListSupportedOptionsResponse> ListSupportedOptionsAsync();

        /// <summary>
        /// Lists the configurable UCI options supported by the engine.
        /// </summary>
        /// <param name="cancellationToken">A custom cancellation token.</param>
        /// <returns>A response object containing all the UCI options.</returns>
        Task<ListSupportedOptionsResponse> ListSupportedOptionsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Connect to the server.
        /// </summary>
        /// <param name="request">The <see cref="StartEvaluationRequest" /> request.</param>
        /// <returns>For quick evaluations, returns the score, best move, and lines.</returns>
        Task<StartEvaluationResponse> StartEvaluationAsync(StartEvaluationRequest request);

        /// <summary>
        /// Connect to the server.
        /// </summary>
        /// <param name="request">The <see cref="StartEvaluationRequest" /> request.</param>
        /// <param name="cancellationToken">A custom cancellation token.</param>
        /// <returns>For quick evaluations, returns the score, best move, and lines.</returns>
        Task<StartEvaluationResponse> StartEvaluationAsync(StartEvaluationRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Hits the server's health check endpoint.
        /// </summary>
        /// <returns>"svc-up" for a healthy server.</returns>
        Task<string> HealthCheckAsync();

        /// <summary>
        /// Hits the server's health check endpoint.
        /// </summary>
        /// <param name="cancellationToken">A custom cancellation token.</param>
        /// <returns>"svc-up" for a healthy server.</returns>
        Task<string> HealthCheckAsync(CancellationToken cancellationToken);
    }
}