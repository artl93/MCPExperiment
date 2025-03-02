namespace Microsoft.Extensions.MCP.Models
{
    /// <summary>
    /// Describes an argument accepted by a prompt.
    /// 
    /// API doc reference: see MCPSchema.ts.txt for details.
    /// </summary>
    public class PromptArgument
    {
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public bool? Required { get; set; }
    }
}
