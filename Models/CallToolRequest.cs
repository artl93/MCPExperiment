using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.ModelContextProtocol.Models
{
    /// <summary>
    /// Used by the client to invoke a tool provided by the server.
    /// Corresponds to the TypeScript CallToolRequest.
    /// </summary>
    public class CallToolRequest : Request
    {
        public CallToolRequest()
        {
            Method = "tools/call";
        }

        public new CallToolParams Params { get; set; } = new CallToolParams();
    }

    /// <summary>
    /// Parameters for CallToolRequest.
    /// 
    /// name: The tool name.
    /// arguments: Expected parameters according to the JSON Schema.
    /// </summary>
    public class CallToolParams
    {
        public string Name { get; set; } = "";
        
        [JsonPropertyName("arguments")]
        public Dictionary<string, object>? Arguments { get; set; }
    }
}
