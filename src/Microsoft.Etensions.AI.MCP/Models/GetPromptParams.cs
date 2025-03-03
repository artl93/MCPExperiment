using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Extensions.AI.MCP.Messages;

namespace Microsoft.Extensions.AI.MCP.Models
{
    /// <summary>
    /// Parameters for GetPromptRequest including prompt name and arguments.
    /// 
    /// API doc reference: see MCPSchema.ts.txt for details.
    /// </summary>
    public class GetPromptParams : IClientRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        
        /// <summary>
        /// Added alias property for compatibility with existing code
        /// </summary>
        public string PromptName
        {
            get => Name;
            set => Name = value;
        }
        
        [JsonPropertyName("arguments")]
        public Dictionary<string, string>? Arguments { get; set; }
        
        /// <summary>
        /// Added alias property for compatibility with existing code
        /// </summary>
        public Dictionary<string, string>? Parameters
        {
            get => Arguments;
            set => Arguments = value;
        }
    }
}
