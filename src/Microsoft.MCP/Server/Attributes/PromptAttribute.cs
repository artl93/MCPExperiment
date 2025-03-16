using System;

namespace Microsoft.MCP.Server.Attributes
{
    /// <summary>
    /// Attribute for marking methods as MCP prompts.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PromptAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the prompt. If not specified, the method name is used.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the prompt.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the category of the prompt.
        /// </summary>
        public string Category { get; set; } = string.Empty;
    }
}