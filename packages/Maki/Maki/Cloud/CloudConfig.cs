//-----------------------------------------------------------------------
// <copyright file="CloudConfig.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Cloud
{
    using Amazon;
    using Amazon.Runtime;

    /// <summary>
    /// A configuration for working with AWS services.
    /// </summary>
    public readonly struct CloudConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloudConfig"/> struct.
        /// </summary>
        /// <param name="region">The direction of the sequence.</param>
        /// <param name="creds">The array of squares.</param>
        public CloudConfig(RegionEndpoint region, AWSCredentials creds)
        {
            this.Creds = creds;
            this.Region = region;
        }

        /// <summary>
        /// Gets the AWS credentials associated with this CloudConfig.
        /// </summary>
        public AWSCredentials Creds { get; }

        /// <summary>
        /// Gets the region.
        /// </summary>
        public RegionEndpoint Region { get; }

        /// <summary>
        /// Create a new AWS service client.
        /// </summary>
        /// <typeparam name="T">The interface of the client to create.</typeparam>
        /// <returns>The service client.</returns>
        public T CreateAWSClient<T>()
            where T : IAmazonService
        {
            return CloudClientFactory.CreateClient<T>(this.Creds, this.Region);
        }

        /// <summary>
        /// Create a new AWS service client with an overriden region.
        /// </summary>
        /// <param name="region">The overriden AWS region.</param>
        /// <typeparam name="T">The interface of the client to create.</typeparam>
        /// <returns>The service client.</returns>
        public T CreateAWSClient<T>(RegionEndpoint region)
            where T : IAmazonService
        {
            return CloudClientFactory.CreateClient<T>(this.Creds, region);
        }
    }
}