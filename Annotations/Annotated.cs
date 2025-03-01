using System.Text.Json.Serialization;

namespace Microsoft.Extensions.ModelContextProtocol.Annotations
{
    public class Annotated
    {
        [JsonPropertyName("annotations")]
        public Annotations Annotations { get; set; } = default!;
    }
}
