using System.Collections.Generic;
using Microsoft.Extensions.AI.MCP.Capabilities;

namespace Microsoft.Extensions.AI.MCP.Server
{
    /// <summary>
    /// Options for configuring the MCP Server.
    /// </summary>
    public class MCPServerOptions
    {
        /// <summary>
        /// Gets or sets the server capabilities.
        /// </summary>
        public ServerCapabilities ServerCapabilities { get; set; } = new ServerCapabilities();

        /// <summary>
        /// Gets or sets whether to enable stdin/stdout protocol negotiation.
        /// </summary>
        public bool EnableStdioProtocol { get; set; } = false;

        /// <summary>
        /// Gets or sets the protocol version to use.
        /// </summary>
        public string ProtocolVersion { get; set; } = Constants.LatestProtocolVersion;

        /// <summary>
        /// Gets or sets custom experimental capabilities.
        /// </summary>
        public Dictionary<string, object> ExperimentalCapabilities { get; set; } = new Dictionary<string, object>();
    }
}