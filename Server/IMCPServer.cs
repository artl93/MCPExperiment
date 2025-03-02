using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.AI.MCP.Initialization;

namespace Microsoft.Extensions.AI.MCP.Server
{
    /// <summary>
    /// Interface for MCP server implementations.
    /// </summary>
    public interface IMCPServer
    {
        /// <summary>
        /// Initializes the server with the specified client capabilities.
        /// </summary>
        /// <param name="request">The initialize request from the client.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The initialization result.</returns>
        Task<InitializeResult> InitializeAsync(InitializeRequest request, CancellationToken cancellationToken = default);

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