//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Amazon;
    using Amazon.Runtime;
    using Amazon.Runtime.CredentialManagement;
    using Maki.Cloud;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The entry point class for the fish wrap daemon.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry point method.
        /// </summary>
        /// <param name="args">Command line args.</param>
        public static void Main(string[] args)
        {
            var cloudConfig = CreateCloudConfig(args);
            using var host = CreateHostBuilder(cloudConfig)
                .Build();
            host.Run();
        }

        /// <summary>
        /// Creates a host builder that will setup and run the application.
        /// </summary>
        /// <param name="cc">The cloud config.</param>
        /// <returns>The host builder.</returns>
        public static IHostBuilder CreateHostBuilder(CloudConfig cc) =>
            Host
                .CreateDefaultBuilder()
                .ConfigureWebHostDefaults((builder) => ConfigureWebHost(builder, cc))
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    logging.AddAWSProvider();

                    // When you need logging below set the minimum level. Otherwise the logging framework will default to Informational for external providers.
                    logging.SetMinimumLevel(LogLevel.Trace);
                });

        /// <summary>
        /// Configures the web host settings to bind to all available IPs
        /// (for some reason without this, in Docker it binds to localhost),
        /// and to use CloudWatch logs.
        /// </summary>
        /// <param name="builder">The web host builder.</param>
        /// <param name="cloudConfig">The cloud config.</param>
        public static void ConfigureWebHost(IWebHostBuilder builder, CloudConfig cloudConfig)
        {
            builder.UseConfiguration(CreateConfig(cloudConfig));
            builder.UseUrls("http://0.0.0.0:5000");
            builder.UseStartup<Startup>();
        }

        /// <summary>
        /// Creates the configuration root from the given cloud config.
        /// </summary>
        /// <param name="cloudConfig">The cloud config.</param>
        /// <returns>The config root to use for the asp.net core configuration.</returns>
        internal static IConfigurationRoot CreateConfig(CloudConfig cloudConfig)
        {
            var logConfig = new CloudLogsConfig()
            {
                Region = cloudConfig.Region,
                LogGroup = "Maki",
            };
            logConfig.SetNamespaceLogLevel(typeof(Program).Namespace, LogLevel.Debug);
            logConfig.SetNamespaceLogLevel("Microsoft", LogLevel.Information);
            logConfig.SetNamespaceLogLevel("System", LogLevel.Information);

            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(logConfig.ToConfigDictionary());
            return configBuilder.Build();
        }

        /// <summary>
        /// Create a CloudConfig object based on command line args.
        /// </summary>
        /// <param name="args">The command-line args.</param>
        /// <returns>The CloudConfig.</returns>
        /// <exception cref="ArgumentException">If a command-line arg is not given in a valid format.</exception>
        internal static CloudConfig CreateCloudConfig(string[] args)
        {
            string allArgs = string.Join(string.Empty, args);
            if (allArgs == "-i")
            {
                Console.WriteLine("Running in interactive mode...");
                var chain = new CredentialProfileStoreChain();
                AWSCredentials awsCredentials;
                if (!chain.TryGetAWSCredentials("chessdb-staging", out awsCredentials))
                {
                    throw new ArgumentException($"CloudConfig cannot be initialized. Interactive mode was specified but the " +
                                                "'chessdb-staging' AWS CLI profile cannot be found on this machine.");
                }

                return new CloudConfig(RegionEndpoint.USEast2, awsCredentials);
            }
            else
            {
                var creds = FallbackCredentialsFactory.GetCredentials();
                var region = FallbackRegionFactory.GetRegionEndpoint();
                return new CloudConfig(region, creds);
            }
        }
    }
}