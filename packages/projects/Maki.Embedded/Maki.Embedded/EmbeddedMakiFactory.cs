//-----------------------------------------------------------------------
// <copyright file="EmbeddedMakiFactory.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Embedded
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Maki.Cloud;
    using Maki.Engine;
    using Maki.IO;
    using Maki.Model.Serialization;
    using Maki.Serialization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// A class to create embedded Maki server instances.
    /// </summary>
    public class EmbeddedMakiFactory
    {
        /// <summary>
        /// Creates a new embedded Maki instance.
        /// </summary>
        /// <returns>The embedded Maki instance.</returns>
        public static EmbeddedMaki Create()
        {
            string tmpPath = Path.GetTempFileName();
            File.Delete(tmpPath);
            var cloudConfig = Program.CreateCloudConfig(new string[0]);
            var hostBuilder = CreateHostBuilder(cloudConfig);
            var host = hostBuilder.Build();
            return new EmbeddedMaki(host);
        }

        private static IHostBuilder CreateHostBuilder(CloudConfig cc) => Host
            .CreateDefaultBuilder()
            .ConfigureWebHostDefaults((builder) => ConfigureWebHost(builder, cc))
            .ConfigureLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Trace);
            });

        private static void ConfigureWebHost(IWebHostBuilder builder, CloudConfig cloudConfig)
        {
            builder.Configure((app) =>
            {
                var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
                var portOracle = app.ApplicationServices.GetRequiredService<PortOracle>();
                lifetime.ApplicationStarted.Register(() =>
                {
                    var addressFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
                    var uris = addressFeature.Addresses.Select(uri => new Uri(uri)).ToList();
                    portOracle.Uris = uris;
                });
                app.UseRouting();
                app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

                // I can add a note here to remind me what this does
                var webSocketOptions = new WebSocketOptions()
                {
                    KeepAliveInterval = TimeSpan.FromSeconds(120),
                };
                app.UseWebSockets(webSocketOptions);
                app.MapWebSocketManager("/ws", app.ApplicationServices.GetService<EngineWebSocketHandler>());
            });
            builder.ConfigureServices(svcs =>
            {
                svcs.AddSingleton<PortOracle>();
                svcs.AddControllers(options =>
                {
                    options.InputFormatters.Add(new ProtobufInputFormatter());
                    options.OutputFormatters.Add(new ProtobufOutputFormatter());
                }).AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new TimeSpanConverter());
                });
                svcs.AddSingleton<EngineBridge>();
                svcs.AddWebSocketManager();
            });
            builder.UseUrls("http://[::1]:0;https://[::1]:0");
            builder.UseSetting(WebHostDefaults.ApplicationKey, typeof(Startup).GetTypeInfo().Assembly.FullName);
        }
    }
}