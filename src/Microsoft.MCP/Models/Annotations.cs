namespace Microsoft.MCP.Models
{
    /// <summary>
    /// Represents optional annotations for audience and priority.
    /// 
    /// API doc reference: see MCPSchema.ts.txt for details.
    /// </summary>
    public class Annotations
    {
        public string[]? Audience { get; set; }
        public double? Priority { get; set; }
    }
}
