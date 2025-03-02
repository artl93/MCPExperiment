using Microsoft.AspNetCore.Builder;

namespace Microsoft.Extensions.AI.MCP.Server
{
    /// <summary>
    /// Extensions for adding MCP middleware to the ASP.NET Core pipeline.
    /// </summary>
    public static class MCPMiddlewareExtensions
    {
        /// <summary>
        /// Adds the MCP middleware to the pipeline.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        /// <returns>The application builder.</returns>
        public static IApplicationBuilder UseMCP(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MCPMiddleware>();
        }
    }
}