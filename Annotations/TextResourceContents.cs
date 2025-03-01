using System.Text.Json.Serialization;

namespace Microsoft.Extensions.ModelContextProtocol.Annotations
{
    public class TextResourceContents : ResourceContents
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = default!;
    }
}
