using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Extensions.AI.MCP.Messages;
using Microsoft.Extensions.AI.MCP.Pagination;

namespace Microsoft.Extensions.AI.MCP.Resources
{
    /// <summary>
    /// The server's response to a resources/list request from the client.
    /// </summary>
    public class ListResourcesResult : PaginatedResult, IClientResult
    {
        /// <summary>
        /// Gets or sets the list of resources provided by the server.
        /// </summary>
        [JsonPropertyName("resources")]
        public List<Resource> Resources { get; set; } = default!;
    }
}
