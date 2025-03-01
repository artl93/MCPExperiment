using System.Text.Json.Serialization;

namespace Microsoft.Extensions.ModelContextProtocol.Models
{
    /// <summary>
    /// Base JSON-RPC result.
    /// 
    /// API doc reference: see MCPSchema.ts.txt for details.
    /// </summary>
    public abstract class Result
    {
        [JsonPropertyName("_meta")]
        public object? Meta { get; set; }
    }
}
