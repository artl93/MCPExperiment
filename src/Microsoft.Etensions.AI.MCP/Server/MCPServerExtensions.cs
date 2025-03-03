using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI.MCP.Server;
using Microsoft.Extensions.AI.MCP.Server.SSE;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.AI.MCP
{
    /// <summary>
    /// Extension methods for setting up MCP Server services in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class MCPServerExtensions
    {
        /// <summary>
        /// Adds MCP Server services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        public static IServiceCollection AddMCP(this IServiceCollection services)
        {
            return services.AddMCP(options => { });
        }

        /// <summary>
        /// Adds MCP Server services to the specified <see cref="IServiceCollection" /> with options.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="configureOptions">A delegate to configure the <see cref="MCPServerOptions"/>.</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        public static IServiceCollection AddMCP(this IServiceCollection services, Action<MCPServerOptions> configureOptions)
        {
            services.Configure(configureOptions);
            
            // Register protocol handler
            services.AddSingleton<IMCPProtocolHandler, JsonRpcProtocolHandler>();
            
            // First, add a bootstrap implementation of IMCPServerInitializer that doesn't depend on IMCPServer
            services.AddSingleton<IMCPServerInitializer, MCPServerBootstrap>();
            
            // Register message processor (which now depends on the bootstrap initializer)
            services.AddTransient<IMCPMessageProcessor, MCPMessageProcessor>();
            
            // Now register the real server (depends on MessageProcessor)
            services.AddSingleton<IMCPServer, MCPServer>();
            
            // Register SSE services
            services.AddSingleton<SSEConnectionManager>();
            services.AddSingleton<SSENotificationHandler>();
            
            return services;
        }
        
    }
}