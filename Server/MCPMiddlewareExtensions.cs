using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI.MCP.Server.SSE;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.AI.MCP.Server
{
    /// <summary>
    /// Extensions for adding MCP middleware to the ASP.NET Core pipeline.
    /// </summary>
    public static class MCPMiddlewareExtensions
    {
        /// <summary>
        /// Adds the MCP middleware to the pipeline.
        /// This only sets up the JSON-RPC endpoint at "/mcp".
        /// For full spec support with SSE, use <see cref="UseMCPWithSSE"/> instead.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        /// <returns>The application builder.</returns>
        public static IApplicationBuilder UseMCP(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MCPMiddleware>();
        }
        
        /// <summary>
        /// Adds MCP middleware with full MCP specification support including SSE.
        /// This configures:
        /// - The "/mcp" endpoint for JSON-RPC
        /// - The "/sse" endpoint for Server-Sent Events
        /// - Automatic notification broadcasting
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <returns>The same application builder so that multiple calls can be chained.</returns>
        public static IApplicationBuilder UseMCPWithSSE(this IApplicationBuilder app)
        {
            // Add SSE middleware first to handle the /sse endpoint
            app.UseMiddleware<SSEMiddleware>();
            
            // Then add standard MCP middleware for the /mcp endpoint
            app.UseMiddleware<MCPMiddleware>();
            
            // Set up notification handling for SSE
            var server = app.ApplicationServices.GetRequiredService<IMCPServer>();
            var notificationHandler = app.ApplicationServices.GetRequiredService<SSENotificationHandler>();
            
            // Wire up SSE notification handler to server's notification event
            server.NotificationReady += async (sender, e) => 
                await notificationHandler.HandleNotificationAsync(sender, e);
            
            return app;
        }
    }
}