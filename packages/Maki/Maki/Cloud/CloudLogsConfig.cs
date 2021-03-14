//-----------------------------------------------------------------------
// <copyright file="CloudLogsConfig.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Cloud
{
    using System.Collections.Generic;
    using Amazon;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// A type to store the CloudWatch logs config.
    /// </summary>
    public class CloudLogsConfig
    {
        private readonly Dictionary<string, LogLevel> namespaceLevels;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudLogsConfig"/> class.
        /// </summary>
        public CloudLogsConfig()
        {
            this.namespaceLevels = new Dictionary<string, LogLevel>();
        }

        /// <summary>
        /// Gets or sets the region to send logs to.
        /// </summary>
        public RegionEndpoint Region { get; set; }

        /// <summary>
        /// Gets or sets the name of the log group.
        /// </summary>
        public string LogGroup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the log level should be included.
        /// </summary>
        public bool IncludeLogLevel { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the category should be included.
        /// </summary>
        public bool IncludeCategory { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether newlines should be included.
        /// </summary>
        public bool IncludeNewline { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the exception should be included.
        /// </summary>
        public bool IncludeException { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the event id should be included.
        /// </summary>
        public bool IncludeEventId { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the log scope should be included.
        /// </summary>
        public bool IncludeScopes { get; set; } = true;

        /// <summary>
        /// Sets the log level for a given namespace.
        /// </summary>
        /// <param name="namespace">The namespace to set the log level for.</param>
        /// <param name="logLevel">The log level.</param>
        public void SetNamespaceLogLevel(string @namespace, LogLevel logLevel) =>
            this.namespaceLevels.Add(@namespace, logLevel);

        /// <summary>
        /// Creates a config dictionary that can be used with an
        /// in-memory configuration source.
        /// </summary>
        /// <returns>The config dictionary.</returns>
        public IDictionary<string, string> ToConfigDictionary()
        {
            var config = new Dictionary<string, string>();
            config.Add("Logging:Region", this.Region.SystemName);
            config.Add("Logging:LogGroup", this.LogGroup);
            config.Add("Logging:IncludeLogLevel", this.IncludeLogLevel.ToString());
            config.Add("Logging:IncludeCategory", this.IncludeCategory.ToString());
            config.Add("Logging:IncludeNewline", this.IncludeNewline.ToString());
            config.Add("Logging:IncludeException", this.IncludeException.ToString());
            config.Add("Logging:IncludeEventId", this.IncludeEventId.ToString());
            config.Add("Logging:IncludeScopes", this.IncludeScopes.ToString());
            foreach (var kvp in this.namespaceLevels)
            {
                config.Add($"Logging:LogLevel:{kvp.Key}", kvp.Value.ToString());
            }

            return config;
        }
    }
}