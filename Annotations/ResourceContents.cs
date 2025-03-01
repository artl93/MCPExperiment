using System.Text.Json.Serialization;

namespace Microsoft.Extensions.ModelContextProtocol.Annotations
{
    public class ResourceContents
    {
        [JsonPropertyName("uri")]
        public string Uri { get; set; } = default!;

        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; } = default!;
    }
}
