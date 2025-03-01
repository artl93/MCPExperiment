using System.Text.Json.Serialization;

namespace Microsoft.Extensions.ModelContextProtocol.Capabilities
{
    /// <summary>
    /// Capabilities related to root directories that the client supports.
    /// </summary>
    public class ClientRootsCapabilities
    {
        /// <summary>
        /// Gets or sets a value indicating whether the client supports notifications for changes to the roots list.
        /// </summary>
        [JsonPropertyName("listChanged")]
        public bool? ListChanged { get; set; }
    }
}
