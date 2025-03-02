using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.AI.MCP.Server.Models
{
    /// <summary>
    /// Represents a tool definition.
    /// </summary>
    public class ToolDefinition
    {
        /// <summary>
        /// Gets or sets the name of the tool.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the tool.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the parameters of the tool.
        /// </summary>
        [JsonPropertyName("parameters")]
        public Dictionary<string, ToolParameterDefinition> Parameters { get; set; } = new Dictionary<string, ToolParameterDefinition>();

        /// <summary>
        /// Gets or sets the delegate that handles the tool call.
        /// </summary>
        [JsonIgnore]
        public Delegate Handler { get; set; } = null!;

        /// <summary>
        /// Gets or sets whether the tool is synchronous.
        /// </summary>
        [JsonIgnore]
        public bool IsSync { get; set; }
    }

    /// <summary>
    /// Represents a tool parameter definition.
    /// </summary>
    public class ToolParameterDefinition
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
    }
}