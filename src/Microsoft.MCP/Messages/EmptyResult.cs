using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.MCP;

namespace Microsoft.MCP.Messages
{
    /// <summary>
    /// A response that indicates success but carries no data.
    /// </summary>
    /// <remarks>
    /// This result can be returned by both clients and servers to indicate
    /// success when no additional data is needed.
    /// </remarks>
    public class EmptyResult : Result, IClientResult, IServerResult
    {
        // ...existing code...
    }
}
