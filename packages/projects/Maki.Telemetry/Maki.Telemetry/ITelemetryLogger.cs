// -----------------------------------------------------------------------
// <copyright file="ITelemetryClient.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
// -----------------------------------------------------------------------

namespace Maki.Telemetry
{
    using System.Threading.Tasks;
    using Maki.Telemetry.Model;

    /// <summary>
    /// A client capable of sending telemetry.
    /// </summary>
    public interface ITelemetryLogger
    {
        /// <summary>
        /// Send a log event to the telemetry client.
        /// </summary>
        /// <param name="logEvent">The event to send.</param>
        /// <returns>An awaitable task.</returns>
        Task SendAsync(LogEvent logEvent);
    }
}