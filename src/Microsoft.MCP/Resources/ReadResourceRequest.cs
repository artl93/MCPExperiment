using Microsoft.MCP.Messages;

namespace Microsoft.MCP.Resources
{
    /// <summary>
    /// Sent from the client to the server, to read a specific resource URI.
    /// </summary>
    public class ReadResourceRequest : JsonRpcRequest<ReadResourceRequestParams>, IClientRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadResourceRequest"/> class.
        /// </summary>
        public ReadResourceRequest() => Method = "resources/read";
    }
}
