using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.ModelContextProtocol.Capabilities
{
    /// <summary>
    /// Capabilities that a server may support. Known capabilities are defined here, in this schema, 
    /// but this is not a closed set: any server can define its own, additional capabilities.
    /// </summary>
    public class ServerCapabilities
    {
        /// <summary>
        /// Gets or sets experimental, non-standard capabilities that the server supports.
        /// </summary>
        [JsonPropertyName("experimental")]
        public Dictionary<string, object> Experimental { get; set; } = default!;

        /// <summary>
        /// Gets or sets server logging capabilities. Present if the server supports sending log messages to the client.
        /// </summary>
        [JsonPropertyName("logging")]
        public object Logging { get; set; } = default!;

        /// <summary>
        /// Gets or sets server prompt capabilities. Present if the server offers any prompt templates.
        /// </summary>
        [JsonPropertyName("prompts")]
        public ServerPromptsCapabilities Prompts { get; set; } = default!;

        /// <summary>
        /// Gets or sets server resource capabilities. Present if the server offers any resources to read.
        /// </summary>
        [JsonPropertyName("resources")]
        public ServerResourcesCapabilities Resources { get; set; } = default!;

        /// <summary>
        /// Gets or sets server tool capabilities. Present if the server offers any tools to call.
        /// </summary>
        [JsonPropertyName("tools")]
        public ServerToolsCapabilities Tools { get; set; } = default!;
    }
}
