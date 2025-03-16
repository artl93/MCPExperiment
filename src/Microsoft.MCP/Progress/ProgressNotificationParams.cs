using System.Text.Json.Serialization;
using Microsoft.MCP.Union;

namespace Microsoft.MCP.Progress
{
    /// <summary>
    /// Parameters for a progress notification.
    /// </summary>
    public class ProgressNotificationParams
    {
        /// <summary>
        /// The progress token which was given in the initial request, used to associate this notification with the request that is proceeding.
        /// </summary>
        [JsonPropertyName("progressToken")]
        public UnionValue ProgressToken { get; set; } = default!;

        /// <summary>
        /// The progress thus far. This should increase every time progress is made, even if the total is unknown.
        /// </summary>
        [JsonPropertyName("progress")]
        public double Progress { get; set; }

        /// <summary>
        /// Total number of items to process (or total progress required), if known.
        /// </summary>
        [JsonPropertyName("total")]
        public double? Total { get; set; }
    }
}
