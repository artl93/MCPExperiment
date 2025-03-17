using System.Threading;
using System.Threading.Tasks;

namespace MCPExperiment.Server
{
    /// <summary>
    /// Interface for MCP message processors.
    /// </summary>
    public interface IMCPMessageProcessor
    {
        /// <summary>
        /// Processes a deserialized message.
        /// </summary>
        /// <param name="message">The deserialized message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The response message, or null if no response is needed.</returns>
        Task<object?> ProcessMessageAsync(object message, CancellationToken cancellationToken = default);
    }
}