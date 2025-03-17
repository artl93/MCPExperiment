using System.Collections.Generic;
using System.Text.Json.Serialization;
using MCPExperiment.Messages;
using MCPExperiment.Pagination;

namespace MCPExperiment.Resources
{
    /// <summary>
    /// The server's response to a resources/templates/list request from the client.
    /// </summary>
    public class ListResourceTemplatesResult : PaginatedResult, IClientResult
    {
        /// <summary>
        /// Gets or sets the list of resource templates provided by the server.
        /// </summary>
        [JsonPropertyName("resourceTemplates")]
        public List<ResourceTemplate> ResourceTemplates { get; set; } = default!;
    }
}
