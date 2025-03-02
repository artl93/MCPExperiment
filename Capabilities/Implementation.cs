using System.Text.Json.Serialization;

namespace Microsoft.Extensions.MCP.Capabilities
{
    /// <summary>
    /// Describes the name and version of an MCP implementation.
    /// </summary>
    public class Implementation
    {
        /// <summary>
        /// Gets or sets the name of the MCP implementation.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        /// <summary>
        /// Gets or sets the version of the MCP implementation.
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; } = default!;
    }
}
