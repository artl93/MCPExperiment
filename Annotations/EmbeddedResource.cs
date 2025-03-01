using System.Text.Json.Serialization;

namespace Microsoft.Extensions.ModelContextProtocol.Annotations
{
    /// <summary>
    /// Represents an embedded resource in the context.
    /// </summary>
    /// <remarks>
    /// An embedded resource can reference content via URI or contain inline content.
    /// </remarks>
    public class EmbeddedResource : Annotated
    {
        /// <summary>
        /// Gets or sets the type of the content. Always "resource" for embedded resources.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = "resource";

        /// <summary>
        /// Gets or sets the resource contents.
        /// </summary>
        [JsonPropertyName("resource")]
        public ResourceContents Resource { get; set; } = default!;
    }
}
