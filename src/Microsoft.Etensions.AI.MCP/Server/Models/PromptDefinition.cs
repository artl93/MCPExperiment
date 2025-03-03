using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.AI.MCP.Server.Models
{
    /// <summary>
    /// Represents a prompt definition.
    /// </summary>
    public class PromptDefinition
    {
        /// <summary>
        /// Gets or sets the name of the prompt.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the prompt.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the category of the prompt.
        /// </summary>
        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the parameters of the prompt.
        /// </summary>
        [JsonPropertyName("parameters")]
        public Dictionary<string, PromptParameterDefinition> Parameters { get; set; } = new Dictionary<string, PromptParameterDefinition>();

        /// <summary>
        /// Gets or sets the delegate that handles the prompt call.
        /// </summary>
        [JsonIgnore]
        public Delegate Handler { get; set; } = null!;
    }

    /// <summary>
    /// Represents a prompt parameter definition.
    /// </summary>
    public class PromptParameterDefinition
    {
        /// <summary>
        /// Gets or sets the description of the parameter.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of the parameter.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the parameter is required.
        /// </summary>
        [JsonPropertyName("required")]
        public bool Required { get; set; } = true;

        /// <summary>
        /// Gets or sets the default value for the parameter.
        /// </summary>
        [JsonPropertyName("defaultValue")]
        public string? DefaultValue { get; set; }
    }
}