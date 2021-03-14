// -----------------------------------------------------------------------
// <copyright file="MetricDirective.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
// -----------------------------------------------------------------------

namespace Maki.Telemetry.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// A MetricDirective class.
    /// </summary>
    public class MetricDirective
    {
        /// <summary>
        /// Gets or sets the metric namespace.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the list of dimensions.
        /// </summary>
        public List<string> Dimensions { get; set; }

        /// <summary>
        /// Gets or sets the list of metrics.
        /// </summary>
        public List<MetricDefinition> Metrics { get; set; }
    }
}