using System;
using System.Threading.Tasks;
using MCPExperiment;
using MCPExperiment.Annotations;
using MCPExperiment.Models;
using MCPExperiment.Server;
using MCPExperiment.TestApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MCPExperiment.TestApp.StdioMCP
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
            // Map tools and prompts BEFORE configuring the HTTP pipeline
            // This ensures tools are registered before SSE connections are established
            host.MapTool<WeatherRequest, WeatherResponse>(
                "getWeather",
                "Gets the weather for a specific location",
                async (request) => await SharedImplementations.GetWeatherAsync(request));
            
            host.MapSyncTool<CalculatorRequest, int>(
                "calculate",
                "Performs a calculation",
                (request) => SharedImplementations.CalculateResult(request));
            
            host.MapPrompt<GreetingRequest, PromptMessage[]>(
                "greeting",
                "Generates a greeting message",
                "Messaging",
                async (request) => await SharedImplementations.GenerateGreetingAsync(request));
                
            // Get the server instance
            var server = host.Services.GetRequiredService<IMCPServer>();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            
            logger.LogInformation("Starting MCP STDIO Server...");
            logger.LogInformation("STDIO MCP Server is starting...");
            logger.LogInformation("Waiting for JSON-RPC messages on standard input...");
            
            // Add tool service registration
            var serviceProvider = host.Services;
            // var server = serviceProvider.GetService<IMCPServer>();
            
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