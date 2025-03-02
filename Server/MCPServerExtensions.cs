using System;
using Microsoft.Extensions.AI.MCP.Server;
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
            services.AddSingleton<IMCPServer, MCPServer>();
            services.AddSingleton<IMCPProtocolHandler, JsonRpcProtocolHandler>();
            services.AddTransient<IMCPMessageProcessor, MCPMessageProcessor>();
            
            return services;
        }
    }
}