using System.Text.Json.Serialization;

namespace Microsoft.MCP.Models
{
    /// <summary>
    /// Base JSON-RPC notification.
    /// 
    /// API doc reference: see MCPSchema.ts.txt for details.
    /// </summary>
    public abstract class Notification
    {
        public string Method { get; set; } = "";
        public object? Params { get; set; }
    }
}
