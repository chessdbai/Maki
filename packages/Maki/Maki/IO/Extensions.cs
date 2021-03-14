//-----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.IO
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Extension methods for the websocket middleware.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Map a websocket manager to a particular path.
        /// </summary>
        /// <param name="app">The app builder.</param>
        /// <param name="path">The path to map the manager to.</param>
        /// <param name="handler">The websocket handler.</param>
        /// <returns>The app builder with the websocket manager mapped.</returns>
        /// <exception cref="ArgumentException">Thrown if handler is null.</exception>
        public static IApplicationBuilder MapWebSocketManager(
            this IApplicationBuilder app,
            PathString path,
            WebSocketHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentException($"Argument 'handler' of type WebSocketHandler was null.");
            }

            return app.Map(path, (app) => app.UseMiddleware<WebSocketManagerMiddleware>(handler));
        }

        /// <summary>
        /// Add a websocket connection manager to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection with the connection manager added.</returns>
        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddTransient<WebSocketConnectionManager>();

            var assemblies = AssemblyLoadContext.Default!.Assemblies
                .Where(assembly => !assembly.IsDynamic)
                .SelectMany(assembly => assembly.ExportedTypes)
                .ToList();
            foreach (var type in assemblies)
            {
                if (type.GetTypeInfo().BaseType == typeof(WebSocketHandler))
                {
                    services.AddSingleton(type);
                }
            }

            return services;
        }
    }
}