using System.Text.Json.Serialization;

namespace Microsoft.Extensions.MCP.Pagination
{
    /// <summary>
    /// Parameters for a paginated request.
    /// </summary>
    public class PaginatedRequestParams
    {
        /// <summary>
        /// An opaque token representing the current pagination position.
        /// If provided, the server should return results starting after this cursor.
        /// </summary>
        [JsonPropertyName("cursor")]
        public string? Cursor { get; set; }
    }
}
