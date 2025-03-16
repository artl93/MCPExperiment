using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.MCP.Messages;

namespace Microsoft.MCP.Models
{
    /// <summary>
    /// Used by the client to invoke a tool provided by the server.
    /// Corresponds to the TypeScript CallToolRequest.
    /// </summary>
    public class CallToolRequest : Request, IClientRequest
    {
        public CallToolRequest()
        {
            Method = "tools/call";
        }

        public new CallToolParams Params { get; set; } = new CallToolParams();
        
        /// <summary>
        /// Added property for compatibility with existing code
        /// </summary>
        public string Name
        {
            get => Params.Name;
            set => Params.Name = value;
        }
        
        /// <summary>
        /// Added property for compatibility with existing code
        /// </summary>
        public Dictionary<string, object>? Arguments
        {
            get => Params.Arguments;
            set => Params.Arguments = value;
        }
    }

    /// <summary>
    /// Parameters for CallToolRequest.
    /// 
    /// name: The tool name.
    /// arguments: Expected parameters according to the JSON Schema.
    /// </summary>
    public class CallToolParams
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        
        [JsonPropertyName("arguments")]
        public Dictionary<string, object>? Arguments { get; set; }
    }
}
