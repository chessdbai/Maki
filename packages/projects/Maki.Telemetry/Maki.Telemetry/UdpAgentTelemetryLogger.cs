// -----------------------------------------------------------------------
// <copyright file="UdpAgentTelemetryLogger.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
// -----------------------------------------------------------------------

namespace Maki.Telemetry
{
    using System;
    using System.Net.Sockets;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Maki.Telemetry.Model;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// A UdpAgentTelemetryLogger class.
    /// </summary>
    public class UdpAgentTelemetryLogger : ITelemetryLogger, IDisposable
    {
        private const ushort DefaultPort = 25888;

        private readonly UdpClient client;
        private readonly ILogger<UdpAgentTelemetryLogger> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpAgentTelemetryLogger"/> class.
        /// </summary>
        /// <param name="hostOrIp">The hostname or IP address.</param>
        /// <param name="port">The port to send the event to.</param>
        /// <param name="logger">The logger.</param>
        public UdpAgentTelemetryLogger(
            string hostOrIp,
            ushort port,
            ILogger<UdpAgentTelemetryLogger> logger = null)
        {
            this.client = new UdpClient(hostOrIp, port);
            this.logger = logger ?? new NullLogger<UdpAgentTelemetryLogger>();
        }

        /// <inheritdoc cref="ITelemetryLogger" />
        public async Task SendAsync(LogEvent logEvent)
        {
            var datagram = JsonSerializer.SerializeToUtf8Bytes(logEvent);
            await this.client.SendAsync(datagram, datagram.Length);
        }

        /// <inheritdoc cref="IDisposable" />
        public void Dispose()
        {
            try
            {
                this.client.Dispose();
            }
            catch (Exception e)
            {
                this.logger.LogWarning("Failed to dispose UDP client: {E}", e);
            }
        }
    }
}