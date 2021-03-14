//-----------------------------------------------------------------------
// <copyright file="Startup.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki
{
    using System;
    using System.Text.Json;
    using Maki.Engine;
    using Maki.IO;
    using Maki.Model.Serialization;
    using Maki.Serialization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// The startup class to configure the web application.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.InputFormatters.Add(new ProtobufInputFormatter());
                options.OutputFormatters.Add(new ProtobufOutputFormatter());
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new TimeSpanConverter());
            });
            services.AddSingleton<EngineBridge>();
            services.AddWebSocketManager();
        }

        /// <summary>
        /// This method configures the application and host environment.
        /// </summary>
        /// <param name="app">The app builder.</param>
        /// <param name="env">The web host environment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
            };
            app.UseWebSockets(webSocketOptions);
            app.MapWebSocketManager("/ws", app.ApplicationServices.GetService<EngineWebSocketHandler>());
        }
    }
}