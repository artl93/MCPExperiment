using System.Text.Json.Serialization;

namespace Microsoft.Extensions.ModelContextProtocol.Annotations
{
    public class ImageContent : Annotated
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "image";

        [JsonPropertyName("data")]
        public string Data { get; set; } = default!;

        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; } = default!;
    }
}
