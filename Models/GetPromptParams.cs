using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.ModelContextProtocol.Models
{
    /// <summary>
    /// Parameters for GetPromptRequest including prompt name and arguments.
    /// 
    /// API doc reference: see MCPSchema.ts.txt for details.
    /// </summary>
    public class GetPromptParams
    {
        public string Name { get; set; } = "";
        
        [JsonPropertyName("arguments")]
        public Dictionary<string, string>? Arguments { get; set; }
    }
}
