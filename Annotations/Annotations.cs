using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.ModelContextProtocol.Annotations
{
    public class Annotations
    {
        [JsonPropertyName("audience")]
        public List<Role> Audience { get; set; } = default!;

        [JsonPropertyName("priority")]
        public double? Priority { get; set; }
    }
}
