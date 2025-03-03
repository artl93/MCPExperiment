using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.AI.MCP.Models
{
    /// <summary>
    /// Used by the client to get a prompt provided by the server.rver.
    /// Corresponds to the TypeScript GetPromptRequest.
    /// </summary>ference: see MCPSchema.ts.txt for details.
    public class GetPromptRequest : Request
    {
        public GetPromptRequest() 
        { 
           Method = "prompts/get"; 
        }
        // Using new keyword to specialize the Params type.
        public new GetPromptParams Params { get; set; } = new GetPromptParams();
    }
}
