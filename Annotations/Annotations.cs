using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.ModelContextProtocol.Annotations
{
    /// <summary>
    /// Metadata that can be applied to content.
    /// </summary>
    /// <remarks>
    /// Annotations provide additional context about the content, such as intended audience and priority.
    /// </remarks>
    public class Annotations
    {
        /// <summary>
        /// Gets or sets the intended audience for the content.
        /// </summary>
        /// <remarks>
        /// Specifies which participant roles (user, assistant) should process this content.
        /// </remarks>
        [JsonPropertyName("audience")]
        public List<Role> Audience { get; set; } = default!;

        /// <summary>
        /// Gets or sets the priority of the content.
        /// </summary>
        /// <remarks>
        /// A higher number indicates higher priority. Can be null if no specific priority is set.
        /// </remarks>
        [JsonPropertyName("priority")]
        public double? Priority { get; set; }
    }
}
