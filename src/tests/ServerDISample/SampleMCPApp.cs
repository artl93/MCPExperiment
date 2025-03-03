using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI.MCP;
using Microsoft.Extensions.AI.MCP.Annotations;
using Microsoft.Extensions.AI.MCP.Models;
using Microsoft.Extensions.AI.MCP.Server;
using Microsoft.Extensions.AI.MCP.Server.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SampleMCPApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // If command-line argument "stdio" is provided, run as stdio server
            bool useStdio = args.Contains("--stdio");
            
            if (useStdio)
            {
                await RunAsStdioServer();
            }
            else
            {
                await RunAsWebServer();
            }
        }

        private static async Task RunAsStdioServer()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    // logging.AddFile("mcp-stdio.log"); // Note: This would require a file logging package
                })
                .ConfigureServices(services =>
                {
                    services.AddMCP(options =>
                    {
                        options.EnableStdioProtocol = true;
                    });
                })
                .Build();

            await host.RunMCPConsoleAsync();
        }

        private static async Task RunAsWebServer()
        {
            var builder = WebApplication.CreateBuilder();
            
            // Add services to the container
            builder.Services.AddMCP();
            builder.Logging.AddConsole();
            
            // Add a diagnostic endpoint to test if the application is responding
            builder.Services.AddEndpointsApiExplorer();
            
            var app = builder.Build();
            
            // Add middleware to log all requests (helps with debugging)
            app.Use(async (context, next) =>
            {
                Console.WriteLine($"Request received: {context.Request.Method} {context.Request.Path}");
                await next();
            });
            
            // Configure the HTTP pipeline with full spec support
            app.UseMCPWithSSE();
            
            // Map tools and prompts using the extension methods
            app.MapTool<WeatherRequest, WeatherResponse>(
                "getWeather",
                "Gets the weather for a specific location",
                async (request) => await GetWeatherAsync(request));
            
            app.MapSyncTool<CalculatorRequest, int>(
                "calculate",
                "Performs a calculation",
                (request) => CalculateResult(request));
            
            app.MapPrompt<GreetingRequest, PromptMessage[]>(
                "greeting",
                "Generates a greeting message",
                "Messaging",
                async (request) => await GenerateGreetingAsync(request));
            
            // Add MCP test/debug endpoint
            app.MapGet("/", () => "MCP Server is running. Try /debug/mcp-status for more information.");
            
            // Add an HTML page with a client-side SSE demo
            app.MapGet("/sse-demo", () => Results.Content(@"
<!DOCTYPE html>
<html>
<head>
    <title>MCP Server-Sent Events Demo</title>
    <style>
        body { font-family: Arial, sans-serif; max-width: 800px; margin: 0 auto; padding: 20px; }
        #events { border: 1px solid #ccc; padding: 10px; height: 300px; overflow-y: auto; margin-bottom: 20px; }
        .event { margin-bottom: 10px; padding: 8px; border-radius: 4px; }
        .event.connected { background-color: #d4edda; }
        .event.normal { background-color: #e2e3e5; }
        .timestamp { color: #666; font-size: 0.8em; }
        h1 { color: #333; }
        button { padding: 8px 16px; background: #007bff; color: white; border: none; cursor: pointer; }
        button:hover { background: #0069d9; }
    </style>
</head>
<body>
    <h1>MCP Server-Sent Events Demo</h1>
    <p>This page demonstrates Server-Sent Events from the MCP server.</p>
    
    <button id=""connect"">Connect to SSE</button>
    <button id=""call-tool"">Call Weather Tool</button>
    <button id=""disconnect"">Disconnect</button>
    
    <h2>Events:</h2>
    <div id=""events""></div>
    
    <script>
        let eventSource = null;
        
        function addEvent(type, data) {
            const eventsDiv = document.getElementById('events');
            const eventDiv = document.createElement('div');
            eventDiv.className = `event ${type}`;
            
            const timestamp = document.createElement('div');
            timestamp.className = 'timestamp';
            timestamp.textContent = new Date().toLocaleTimeString();
            
            const content = document.createElement('pre');
            content.textContent = typeof data === 'object' ? JSON.stringify(data, null, 2) : data;
            
            eventDiv.appendChild(timestamp);
            eventDiv.appendChild(content);
            eventsDiv.appendChild(eventDiv);
            eventsDiv.scrollTop = eventsDiv.scrollHeight;
        }
        
        document.getElementById('connect').addEventListener('click', () => {
            if (eventSource) {
                addEvent('normal', 'Already connected to SSE');
                return;
            }
            
            try {
                // Connect to the SSE endpoint
                eventSource = new EventSource('/sse');
                console.log('Connected to SSE endpoint');
                
                // Listen for connection open
                eventSource.onopen = () => {
                    addEvent('connected', 'Connected to SSE stream');
                };
                
                // Listen for messages
                eventSource.onmessage = (event) => {
                    addEvent('normal', event.data);
                };
                
                // Listen for specific event types
                eventSource.addEventListener('connected', (event) => {
                    addEvent('connected', event.data);
                });
                
                // Listen for errors
                eventSource.onerror = (error) => {
                    addEvent('normal', 'Error occurred: ' + JSON.stringify(error));
                    eventSource.close();
                    eventSource = null;
                };
            } catch (err) {
                addEvent('normal', 'Failed to connect: ' + err.message);
            }
        });
        
        document.getElementById('call-tool').addEventListener('click', async () => {
            try {
                // Call the weather tool to trigger a notification
                const response = await fetch('/mcp', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        jsonrpc: '2.0',
                        id: '12345',
                        method: 'tools/call',
                        params: {
                            name: 'getWeather',
                            arguments: {
                                location: 'Seattle'
                            }
                        }
                    })
                });
                
                const result = await response.json();
                addEvent('normal', 'Tool call result: ' + JSON.stringify(result));
            } catch (err) {
                addEvent('normal', 'Tool call failed: ' + err.message);
            }
        });
        
        document.getElementById('disconnect').addEventListener('click', () => {
            if (eventSource) {
                eventSource.close();
                eventSource = null;
                addEvent('normal', 'Disconnected from SSE');
            } else {
                addEvent('normal', 'Not connected to SSE');
            }
        });
    </script>
</body>
</html>
", "text/html"));
            
            // Get the server instance
            var server = app.Services.GetRequiredService<IMCPServer>();
            Console.WriteLine("Obtained MCP Server instance from DI container");
            
            // Force manual server initialization with a dummy initialize request
            try 
            {
                var initRequest = new Microsoft.Extensions.AI.MCP.Initialization.InitializeRequest
                {
                    Params = new Microsoft.Extensions.AI.MCP.Initialization.InitializeRequestParams
                    {
                        ProtocolVersion = "2024-11-05",
                        Capabilities = new Microsoft.Extensions.AI.MCP.Capabilities.ClientCapabilities(),
                        ClientInfo = new Microsoft.Extensions.AI.MCP.Capabilities.Implementation
                        {
                            Name = "SampleMCPApp",
                            Version = "1.0"
                        }
                    }
                };
                var result = server.InitializeAsync(initRequest).GetAwaiter().GetResult();
                Console.WriteLine($"MCP Server manually initialized with {result.Capabilities.Tools?.AvailableTools?.Count ?? 0} tools and {result.Capabilities.Prompts?.AvailablePrompts?.Count ?? 0} prompts");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize MCP Server: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            Console.WriteLine("MCP Server is running on http://localhost:5000");
            Console.WriteLine("- JSON-RPC API: POST to /mcp");
            Console.WriteLine("- SSE Endpoint: Connect to /sse");
            Console.WriteLine("- SSE Demo Page: Open /sse-demo in your browser");
            
            // Add debugging endpoint to see the server status
            app.MapGet("/debug/mcp-status", () => {
                var tools = MCPRoutingExtensions.GetTools();
                var prompts = MCPRoutingExtensions.GetPrompts();
                return new { 
                    IsInitialized = true,
                    RegisteredTools = tools.Keys,
                    RegisteredPrompts = prompts.Keys
                };
            });
            
            // Add a demo notification endpoint to test SSE
            app.MapGet("/send-notification", (string message) => {
                if (string.IsNullOrEmpty(message)) {
                    message = "Test notification from MCP server at " + DateTime.Now.ToString();
                }
                
                server.SendDemoNotification(message);
                return new { success = true, message = "Notification sent" };
            });
            
            // Start the server
            await app.RunAsync();
        }

        // Tool implementations
        private static async Task<WeatherResponse> GetWeatherAsync(WeatherRequest request)
        {
            // Simulate an async operation
            await Task.Delay(100);
            
            var response = new WeatherResponse
            {
                Temperature = 72,
                Conditions = "Sunny",
                Location = request.Location
            };
            
            return response;
        }

        private static int CalculateResult(CalculatorRequest request)
        {
            return request.Operation switch
            {
                "add" => request.A + request.B,
                "subtract" => request.A - request.B,
                "multiply" => request.A * request.B,
                "divide" => request.B != 0 ? request.A / request.B : 0,
                _ => 0
            };
        }

        // Prompt implementations
        private static async Task<PromptMessage[]> GenerateGreetingAsync(GreetingRequest request)
        {
            // Simulate an async operation
            await Task.Delay(100);
            
            var timeOfDay = DateTime.Now.Hour switch
            {
                >= 5 and < 12 => "morning",
                >= 12 and < 17 => "afternoon",
                _ => "evening"
            };
            
            var formality = request.Formal ? "formal" : "casual";
            var greeting = formality == "formal" ? "Greetings" : "Hey";
            
            var message = $"{greeting} {request.Name}, good {timeOfDay}!";
            
            return new[] 
            {
                new PromptMessage 
                { 
                    Role = "system",
                    Content = new TextContent { Text = message }
                }
            };
        }
    }

    // Data models for tools and prompts
    public class WeatherRequest
    {
        [ToolParameter(Description = "The location to get weather for", Required = true)]
        public string Location { get; set; } = string.Empty;

        [ToolParameter(Description = "The unit of temperature (celsius or fahrenheit)", Required = false)]
        public string Unit { get; set; } = "fahrenheit";
    }

    public class WeatherResponse
    {
        public double Temperature { get; set; }
        public string Conditions { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }

    public class CalculatorRequest
    {
        [ToolParameter(Description = "The first operand")]
        public int A { get; set; }

        [ToolParameter(Description = "The second operand")]
        public int B { get; set; }

        [ToolParameter(Description = "The operation to perform (add, subtract, multiply, divide)")]
        public string Operation { get; set; } = "add";
    }

    public class GreetingRequest
    {
        [PromptParameter(Description = "The name of the person to greet")]
        public string Name { get; set; } = string.Empty;

        [PromptParameter(Description = "Whether to use formal language", Required = false)]
        public bool Formal { get; set; } = false;
    }

    // Alternative example with attributes on methods
    public class ToolExamples
    {
        [Tool(Name = "stringLength", Description = "Counts the number of characters in a string")]
        public int GetStringLength([ToolParameter(Description = "The string to count")] string text)
        {
            return text?.Length ?? 0;
        }

        [Tool(Description = "Converts a string to uppercase")]
        public string ToUpperCase([ToolParameter] string text)
        {
            return text?.ToUpper() ?? string.Empty;
        }

        [Prompt(Name = "askQuestion", Description = "Generates a message asking a question", Category = "Conversation")]
        public PromptMessage[] GenerateQuestion(
            [PromptParameter(Description = "The topic of the question")] string topic,
            [PromptParameter(Description = "The difficulty level", Required = false)] string difficulty = "medium")
        {
            return new[] 
            {
                new PromptMessage 
                { 
                    Role = "system",
                    Content = new TextContent { Text = $"Please ask a {difficulty} question about {topic}." }
                }
            };
        }
    }
}