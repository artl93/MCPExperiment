using Microsoft.Extensions.MCP.Messages;

namespace Microsoft.Extensions.MCP.Initialization
{
    /// <summary>
    /// This request is sent from the client to the server when it first connects, asking it to begin initialization.
    /// </summary>
    public class InitializeRequest : JsonRpcRequest<InitializeRequestParams>, IClientRequest
    {
        /// <summary>
        /// Creates a new instance of the InitializeRequest class.
        /// </summary>
        public InitializeRequest() => Method = "initialize";
    }
}
