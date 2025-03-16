using Microsoft.MCP.Messages;

namespace Microsoft.MCP.Initialization
{
    /// <summary>
    /// Parameters for a ping request.
    /// </summary>
    public class PingRequestParams : IClientRequest, IServerRequest
    {
        // No parameters needed for ping request
    }
}