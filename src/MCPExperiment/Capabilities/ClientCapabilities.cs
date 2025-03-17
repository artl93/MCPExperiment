using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MCPExperiment.Capabilities
{
    /// <summary>
    /// Capabilities a client may support. Known capabilities are defined here, in this schema,
    /// but this is not a closed set: any client can define its own, additional capabilities.
    /// </summary>
    public class ClientCapabilities
    {
        /// <summary>
        /// Gets or sets experimental, non-standard capabilities that the client supports.
        /// </summary>
        [JsonPropertyName("experimental")]
        public Dictionary<string, object> Experimental { get; set; } = default!;

        /// <summary>
        /// Gets or sets client roots capabilities. Present if the client supports listing roots.
        /// </summary>
        [JsonPropertyName("roots")]
        public ClientRootsCapabilities Roots { get; set; } = default!;

        /// <summary>
        /// Gets or sets client sampling capabilities. Present if the client supports sampling from an LLM.
        /// </summary>
        [JsonPropertyName("sampling")]
        public object Sampling { get; set; } = default!;
    }
}
