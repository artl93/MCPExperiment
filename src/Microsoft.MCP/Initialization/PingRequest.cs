using Microsoft.MCP.Messages;

namespace Microsoft.MCP.Initialization
{
    /// <summary>
    /// A ping, issued by either the server or the client, to check that the other party is still alive.
    /// The receiver must promptly respond, or else may be disconnected.
    /// </summary>
    public class PingRequest : JsonRpcRequest<object>, IClientRequest, IServerRequest
    {
        /// <summary>
        /// Creates a new instance of the PingRequest class.
        /// </summary>
        public PingRequest() => Method = "ping";
    }
}
