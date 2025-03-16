using System.Text.Json.Serialization;

namespace Microsoft.MCP.Pagination
{
    /// <summary>
    /// A result that supports pagination.
    /// </summary>
    public class PaginatedResult
    {
        /// <summary>
        /// An opaque token representing the pagination position after the last returned result.
        /// If present, there may be more results available.
        /// </summary>
        [JsonPropertyName("nextCursor")]
        public string? NextCursor { get; set; }
    }
}
