using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Extensions.MCP;

namespace Microsoft.Extensions.MCP.Messages
{
    /// <summary>
    /// Interface for all client result objects that can be returned to the server.
    /// </summary>
    /// <remarks>
    /// Client results include EmptyResult, CreateMessageResult, and ListRootsResult.
    /// </remarks>
    public interface IClientResult { }
}
