using System.Text.Json.Serialization;

namespace Microsoft.Extensions.AI.MCP.Annotations
{
    /// <summary>
    /// Base class for objects that can have annotations.
    /// </summary>
    /// <remarks>
    /// This is a base class for all content types that can be annotated with metadata.
    /// </remarks>
    public class Annotated
    {
        /// <summary>
        /// Gets or sets metadata about the content.
        /// </summary>
        [JsonPropertyName("annotations")]
        public Annotations Annotations { get; set; } = default!;
    }
}
