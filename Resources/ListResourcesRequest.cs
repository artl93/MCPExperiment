using Microsoft.Extensions.ModelContextProtocol.Messages;
using Microsoft.Extensions.ModelContextProtocol.Pagination;

namespace Microsoft.Extensions.ModelContextProtocol.Resources
{
    /// <summary>
    /// Sent from the client to request a list of resources the server has.
    /// </summary>
    public class ListResourcesRequest : JsonRpcRequest<PaginatedRequestParams>, IClientRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListResourcesRequest"/> class.
        /// </summary>
        public ListResourcesRequest() => Method = "resources/list";
    }
}
