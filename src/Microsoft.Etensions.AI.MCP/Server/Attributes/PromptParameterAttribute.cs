using System;

namespace Microsoft.Extensions.AI.MCP.Server.Attributes
{
    /// <summary>
    /// Attribute for describing prompt parameters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
    public class PromptParameterAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the description of the parameter.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the parameter is required.
        /// </summary>
        public bool Required { get; set; } = true;

        /// <summary>
        /// Gets or sets the default value for the parameter.
        /// </summary>
        public string? DefaultValue { get; set; }
    }
}