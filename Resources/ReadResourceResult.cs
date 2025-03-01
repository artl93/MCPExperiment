using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Extensions.ModelContextProtocol.Messages;

namespace Microsoft.Extensions.ModelContextProtocol.Resources
{
    /// <summary>
    /// The server's response to a resources/read request from the client.
    /// </summary>
    public class ReadResourceResult : Result, IClientResult
    {
        /// <summary>
        /// Gets or sets the contents of the resource, which can be either TextResourceContents or BlobResourceContents.
        /// </summary>
        [JsonPropertyName("contents")]
        public List<object> Contents { get; set; } = default!;
    }
}
