using Microsoft.Extensions.AI.MCP.Messages;

namespace Microsoft.Extensions.AI.MCP.Initialization
{
    /// <summary>
    /// Parameters for a ping request.
    /// </summary>
    public class PingRequestParams : IClientRequest, IServerRequest
    {
        // No parameters needed for ping request
    }
}