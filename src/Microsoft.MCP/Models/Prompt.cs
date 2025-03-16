using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.MCP.Annotations;

namespace Microsoft.MCP.Models
{
    /// <summary>
    /// A prompt or prompt template provided by the server.
    /// </summary>
    public class Prompt : Annotated
    {
        /// <summary>
        /// The name of the prompt (e.g., "summarize", "translate").
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// A short description of what this prompt does.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// A list of expected parameters for this prompt template.
        /// If empty, this prompt doesn't accept any parameters.
        /// </summary>
        [JsonPropertyName("arguments")]
        public List<PromptArgument> Arguments { get; set; } = new List<PromptArgument>();

        /// <summary>
        /// The messages that make up this prompt.
        /// </summary>
        [JsonPropertyName("messages")]
        public List<PromptMessage> Messages { get; set; } = new List<PromptMessage>();
    }
}
