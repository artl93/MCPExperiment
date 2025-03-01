using System.Text.Json.Serialization;

namespace Microsoft.Extensions.ModelContextProtocol.Annotations
{
    public class EmbeddedResource : Annotated
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "resource";

        [JsonPropertyName("resource")]
        public ResourceContents Resource { get; set; } = default!;
    }
}
