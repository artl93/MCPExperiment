using System;

namespace Microsoft.Extensions.AI.MCP.Server.Attributes
{
    /// <summary>
    /// Attribute for marking methods as MCP tools.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ToolAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the tool. If not specified, the method name is used.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the tool.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the tool is synchronous.
        /// </summary>
        public bool IsSync { get; set; } = false;
    }
}