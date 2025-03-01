using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Extensions.ModelContextProtocol;

namespace Microsoft.Extensions.ModelContextProtocol.Messages
{
    /// <summary>
    /// Interface for all request objects that can be sent from the server to the client.
    /// </summary>
    /// <remarks>
    /// Server requests include PingRequest, CreateMessageRequest, and ListRootsRequest.
    /// </remarks>
    public interface IServerRequest { }
}
