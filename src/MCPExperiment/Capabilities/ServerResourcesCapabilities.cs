using System.Text.Json.Serialization;

namespace MCPExperiment.Capabilities
{
    /// <summary>
    /// Capabilities related to resources that the server supports.
    /// </summary>
    public class ServerResourcesCapabilities
    {
        /// <summary>
        /// Gets or sets a value indicating whether this server supports subscribing to resource updates.
        /// </summary>
        [JsonPropertyName("subscribe")]
        public bool? Subscribe { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this server supports notifications for changes to the resource list.
        /// </summary>
        [JsonPropertyName("listChanged")]
        public bool? ListChanged { get; set; }
    }
}
