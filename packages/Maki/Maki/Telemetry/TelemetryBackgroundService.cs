// -----------------------------------------------------------------------
// <copyright file="TelemetryBackgroundService.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
// -----------------------------------------------------------------------

namespace Maki.Telemetry
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using NvmlLib;

    /// <summary>
    /// A TelemetryBackgroundService class.
    /// </summary>
    public class TelemetryBackgroundService : IHostedService, IDisposable
    {
        private readonly ILogger<TelemetryBackgroundService> logger;
        private readonly NvmlClient nvml;
        private Timer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryBackgroundService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public TelemetryBackgroundService(ILogger<TelemetryBackgroundService> logger)
        {
            this.nvml = new NvmlClient();
            this.logger = logger;
        }

        /// <inheritdoc cref="IHostedService"/>
        public Task StartAsync(CancellationToken stoppingToken)
        {
            this.timer = new Timer(
                this.DoWork,
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        /// <inheritdoc cref="IHostedService"/>
        public Task StopAsync(CancellationToken stoppingToken)
        {
            this.timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        /// <inheritdoc cref="IDisposable"/>
        public void Dispose()
        {
            this.timer?.Dispose();
        }

        private void DoWork(object state)
        {
            var devices = this.nvml.ListAllDevices();
        }
    }
}