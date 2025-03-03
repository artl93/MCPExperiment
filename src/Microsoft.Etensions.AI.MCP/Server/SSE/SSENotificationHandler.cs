using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.AI.MCP.Server.SSE
{
    /// <summary>
    /// Handles server notifications and broadcasts them via SSE.
    /// </summary>
    public class SSENotificationHandler
    {
        private readonly SSEConnectionManager _connectionManager;
        private readonly ILogger<SSENotificationHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SSENotificationHandler"/> class.
        /// </summary>
        /// <param name="connectionManager">The SSE connection manager.</param>
        /// <param name="logger">The logger.</param>
        public SSENotificationHandler(
            SSEConnectionManager connectionManager,
            ILogger<SSENotificationHandler> logger)
        {
            _connectionManager = connectionManager;
            _logger = logger;
        }

        /// <summary>
        /// Handles a server notification and broadcasts it to all connected SSE clients.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        public async Task HandleNotificationAsync(object? sender, ServerNotificationEventArgs e)
        {
            try
            {
                // Parse the notification JSON to extract method
                using var document = JsonDocument.Parse(e.NotificationJson);
                var methodProperty = document.RootElement.GetProperty("method");
                var methodName = methodProperty.GetString() ?? "unknown";
                
                // Create an event name based on the method per the MCP specification
                // The event type should be the method name itself
                string eventName = methodName;
                
                _logger.LogInformation("Handling SSE notification: {Method}", methodName);

                // Send the event to all connected clients
                await _connectionManager.SendEventToAllAsync(eventName, e.NotificationJson);
                
                _logger.LogInformation("Broadcast notification '{Method}' to clients", methodName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error broadcasting notification");
            }
        }
    }
}