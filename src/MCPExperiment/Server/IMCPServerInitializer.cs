using System.Threading;
using System.Threading.Tasks;
using MCPExperiment.Initialization;

namespace MCPExperiment.Server
{
    /// <summary>
    /// Interface for initializing MCP servers. Used to break circular dependency between
    /// MCPServer and MCPMessageProcessor.
    /// </summary>
    public interface IMCPServerInitializer
    {
        /// <summary>
        /// Initializes the server with the specified client capabilities.
        /// </summary>
        /// <param name="request">The initialize request from the client.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The initialization result.</returns>
        Task<InitializeResult> InitializeAsync(InitializeRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a notification to the client.
        /// </summary>
        /// <param name="notification">The notification to send.</param>
        void SendNotification(object notification);
    }
}