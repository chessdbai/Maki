// -----------------------------------------------------------------------
// <copyright file="Metadata.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
// -----------------------------------------------------------------------

namespace Maki.Telemetry.Model
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A Metadata class.
    /// </summary>
    public class Metadata
    {
        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the CloudWatchMetrics.
        /// </summary>
        public List<MetricDirective> CloudWatchMetrics { get; set; } = new List<MetricDirective>();
    }
}