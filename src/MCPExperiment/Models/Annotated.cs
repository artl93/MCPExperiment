namespace MCPExperiment.Models
{
    /// <summary>
    /// Base class that includes optional client annotations.
    /// 
    /// API doc reference: see MCPSchema.ts.txt for details.
    /// </summary>
    public class Annotated
    {
        public Annotations? Annotations { get; set; }
    }
}
