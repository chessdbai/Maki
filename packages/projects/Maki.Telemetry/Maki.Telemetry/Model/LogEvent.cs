// -----------------------------------------------------------------------
// <copyright file="LogEvent.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
// -----------------------------------------------------------------------

namespace Maki.Telemetry.Model
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// A LogEvent class.
    /// </summary>
    public class LogEvent
    {
        /// <summary>
        /// Gets or sets the metadata object.
        /// </summary>
        [JsonPropertyName("_aws")]
        public Metadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the number of nodes explored.
        /// </summary>
        public int Nodes { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes emitted by the engine.
        /// </summary>
        public int EngineOutputBytes { get; set; }

        /// <summary>
        /// Gets or sets the CPU utilization.
        /// </summary>
        public int CPUUtilization { get; set; }

        /// <summary>
        /// Gets or sets the GPU utilization.
        /// </summary>
        public int GPUUtilization { get; set; }

        /// <summary>
        /// Gets or sets the memory utilization.
        /// </summary>
        public int MemoryUtilizationPercent { get; set; }

        /// <summary>
        /// Gets or sets the free memory.
        /// </summary>
        public long FreeMemory { get; set; }

        /// <summary>
        /// Gets or sets the used memory.
        /// </summary>
        public long UsedMemory { get; set; }

        /// <summary>
        /// Gets or sets the tablebase hits.
        /// </summary>
        public int TablebaseHits { get; set; }

    }
}