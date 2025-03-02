using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.AI.MCP.Server.SSE
{
    /// <summary>
    /// Middleware for handling Server-Sent Events connections.
    /// </summary>
    public class SSEMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SSEMiddleware> _logger;
        private readonly SSEConnectionManager _connectionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SSEMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="connectionManager">The SSE connection manager.</param>
        public SSEMiddleware(
            RequestDelegate next,
            ILogger<SSEMiddleware> logger,
            SSEConnectionManager connectionManager)
        {
            _next = next;
            _logger = logger;
            _connectionManager = connectionManager;
        }

        /// <summary>
        /// Processes an HTTP request.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.Equals("/sse", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogDebug("SSE connection request received from {Path}", context.Request.Path);

                // Set up response for SSE
                context.Response.StatusCode = 200;
                
                // Create a new SSE connection
                using var connection = new SSEConnection(context.Response);
                _connectionManager.AddConnection(connection);

                try
                {
                    // Send an initial event to confirm the connection
                    await connection.SendEventAsync("connected", $"{{\"connectionId\":\"{connection.ConnectionId}\"}}");

                    // Keep the connection open until cancelled
                    var tcs = new TaskCompletionSource<object?>();
                    using var registration = connection.CancellationToken.Register(() => tcs.TrySetResult(null));
                    
                    // Wait for cancellation
                    await tcs.Task;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling SSE connection {ConnectionId}", connection.ConnectionId);
                }
                finally
                {
                    _connectionManager.RemoveConnection(connection.ConnectionId);
                }
            }
            else
            {
                // Not an SSE request, continue with the pipeline
                await _next(context);
            }
        }
    }
}