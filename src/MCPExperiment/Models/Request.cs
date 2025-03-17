using System.Text.Json.Serialization;

namespace MCPExperiment.Models
{
    /// <summary>
    /// Base JSON-RPC request.
    /// 
    /// API doc reference: see MCPSchema.ts.txt for details.
    /// </summary>
    public abstract class Request
    {
        public string Method { get; set; } = "";
        public object? Params { get; set; }
    }
}
