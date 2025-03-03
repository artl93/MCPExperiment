using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.AI.MCP.Server
{
    /// <summary>
    /// Interface for MCP server implementations.
    /// </summary>
    public interface IMCPServer : IMCPServerInitializer
    {
        /// <summary>
        /// Processes a raw JSON-RPC message from the client.
        /// </summary>
        /// <param name="message">The raw JSON-RPC message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The response message if any.</returns>
        Task<string?> ProcessMessageAsync(string message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Event raised when the server needs to send a notification to the client.
        /// </summary>
        event EventHandler<ServerNotificationEventArgs> NotificationReady;

        /// <summary>
        /// Starts the stdio protocol handler if enabled.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task StartStdioAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Sends a demo notification for testing SSE functionality.
        /// </summary>
        /// <param name="message">The message to include in the notification.</param>
        void SendDemoNotification(string message);
        
        /// <summary>
        /// Sends a notification to all connected clients informing them about changes to the available tools.
        /// </summary>
        void SendToolListChangedNotification();
    }

    /// <summary>
    /// Event arguments for server notifications.
    /// </summary>
    public class ServerNotificationEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the notification message as JSON.
        /// </summary>
        public string NotificationJson { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ServerNotificationEventArgs"/>.
        /// </summary>
        /// <param name="notificationJson">The notification message as JSON.</param>
        public ServerNotificationEventArgs(string notificationJson)
        {
            NotificationJson = notificationJson;
        }
    }
}