using System.Collections.Generic;
using System.Text.Json.Serialization;
using MCPExperiment;

namespace MCPExperiment.Messages
{
    /// <summary>
    /// Interface for all result objects that can be returned from the server to the client.
    /// </summary>
    /// <remarks>
    /// Server results include EmptyResult, InitializeResult, CompleteResult, GetPromptResult, ListPromptsResult,
    /// ListResourcesResult, ListResourceTemplatesResult, ReadResourceResult, CallToolResult, and ListToolsResult.
    /// </remarks>
    public interface IServerResult { }
}
