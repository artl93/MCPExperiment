using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.AI.MCP.Server
{
    /// <summary>
    /// Extensions for hosting MCP in a console application.
    /// </summary>
    public static class MCPConsoleHostExtensions
    {
        /// <summary>
        /// Runs the MCP server using stdin/stdout protocol.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task RunMCPConsoleAsync(this IHost host, CancellationToken cancellationToken = default)
        {
            // Get the MCP server from DI
            using var scope = host.Services.CreateScope();
            var mcpServer = scope.ServiceProvider.GetRequiredService<IMCPServer>();
            
            // Start the stdio protocol handler
            await mcpServer.StartStdioAsync(cancellationToken);
        }
    }
}