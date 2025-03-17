using MCPExperiment.Messages;
using MCPExperiment.Pagination;

namespace MCPExperiment.Resources
{
    /// <summary>
    /// Sent from the client to request a list of resource templates the server has.
    /// </summary>
    public class ListResourceTemplatesRequest : JsonRpcRequest<PaginatedRequestParams>, IClientRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListResourceTemplatesRequest"/> class.
        /// </summary>
        public ListResourceTemplatesRequest() => Method = "resources/templates/list";
    }
}
