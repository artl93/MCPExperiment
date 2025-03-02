using System;

namespace Microsoft.Extensions.AI.MCP.Server.Attributes
{
    /// <summary>
    /// Attribute for describing tool parameters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class ToolParameterAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the description of the parameter.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the parameter is required.
        /// </summary>
        public bool Required { get; set; } = true;
    }
}