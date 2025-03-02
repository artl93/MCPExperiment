using System;
using System.Threading.Tasks;
using Microsoft.Extensions.AI.MCP;
using Microsoft.Extensions.AI.MCP.Annotations;
using Microsoft.Extensions.AI.MCP.Models;
using Microsoft.Extensions.AI.MCP.Server;
using Microsoft.Extensions.AI.MCP.TestApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.AI.MCP.TestApp.StdioMCP
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await RunStdioServer();
        }

        private static async Task RunStdioServer()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole( o => o.LogToStandardErrorThreshold = LogLevel.Error);
                    // For production, you might want to add file logging
                    // logging.AddFile("mcp-stdio.log");  // This would require a file logging package
                })
                .ConfigureServices(services =>
                {
                    services.AddMCP(options =>
                    {
                        options.EnableStdioProtocol = true;
                    });
                    
                    // Register tool implementations
                    services.AddSingleton<ToolExamples>();
                    
                    // Register the tool handlers explicitly
                    services.AddSingleton<StdioWeatherHandler>();
                    services.AddSingleton<StdioCalculatorHandler>();
                    services.AddSingleton<StdioGreetingHandler>();
                })
                .Build();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            
            logger.LogInformation("Starting MCP STDIO Server...");
            logger.LogInformation("STDIO MCP Server is starting...");
            logger.LogInformation("Waiting for JSON-RPC messages on standard input...");
            
            // Add tool service registration
            var serviceProvider = host.Services;
            var server = serviceProvider.GetService<IMCPServer>();
            
            if (server != null)
            {
                logger.LogInformation("MCP Server obtained, registered tools and prompts will be available");
            }
            
            // No need for direct registration - tools and prompts are auto-discovered
            // from the service provider when using RunMCPConsoleAsync
            
            // Run the console server
            await host.RunMCPConsoleAsync();
        }
    }
}