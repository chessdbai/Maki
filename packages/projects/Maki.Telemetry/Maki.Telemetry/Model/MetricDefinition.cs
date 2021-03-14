// -----------------------------------------------------------------------
// <copyright file="MetricDefinition.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
// -----------------------------------------------------------------------

namespace Maki.Telemetry.Model
{
    /// <summary>
    /// A MetricDefinition class.
    /// </summary>
    public class MetricDefinition
    {
        /// <summary>
        /// Gets or sets the name of the metric.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the unit of the metric.
        /// </summary>
        public string Unit { get; set; }
    }
}