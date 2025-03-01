using System.Text.Json.Serialization;

namespace Microsoft.Extensions.ModelContextProtocol.Annotations
{
    public class BlobResourceContents : ResourceContents
    {
        [JsonPropertyName("blob")]
        public string Blob { get; set; } = default!;
    }
}
