using System.Text.Json.Serialization;

namespace Microsoft.Extensions.ModelContextProtocol.Capabilities
{
    /// <summary>
    /// Capabilities related to prompts that the server supports.
    /// </summary>
    public class ServerPromptsCapabilities
    {
        /// <summary>
        /// Gets or sets a value indicating whether this server supports notifications for changes to the prompt list.
        /// </summary>
        [JsonPropertyName("listChanged")]
        public bool? ListChanged { get; set; }
    }
}
