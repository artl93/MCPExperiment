using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.MCP.Server
{
    /// <summary>
    /// Interface for MCP protocol handlers.
    /// </summary>
    public interface IMCPProtocolHandler
    {
        /// <summary>
        /// Deserializes a message from a JSON string.
        /// </summary>
        /// <param name="json">The JSON string.</param>
        /// <returns>The deserialized message.</returns>
        object DeserializeMessage(string json);

        /// <summary>
        /// Serializes a message to a JSON string.
        /// </summary>
        /// <param name="message">The message to serialize.</param>
        /// <returns>The serialized JSON string.</returns>
        string SerializeMessage(object message);

        /// <summary>
        /// Starts the protocol handler for stdin/stdout.
        /// </summary>
        /// <param name="processor">The message processor to handle incoming messages.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task StartStdioAsync(IMCPMessageProcessor processor, CancellationToken cancellationToken = default);
    }
}