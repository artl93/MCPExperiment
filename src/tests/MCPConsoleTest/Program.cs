using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using MCP;
using Microsoft.MCP.TestApp;

namespace MCPConsoleTest
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.Error.WriteLine("Starting MCP Console Test Server...");
            await RunMCPServer();
        }

        private static async Task RunMCPServer()
        {
            // Create the MCP server directly (no DI)
            var server = new McpServer("MCPConsoleTest", "1.0");
            
            // Register a simple weather tool
            server.AddTool(
                "getWeather", 
                "Gets the current weather for a location",
                (args, ctx) => 
                {
                    string location = args.TryGetValue("location", out var locElem) && locElem.ValueKind == JsonValueKind.String 
                        ? locElem.GetString() ?? "Unknown" 
                        : "Unknown";
                        
                    string unit = args.TryGetValue("unit", out var unitElem) && unitElem.ValueKind == JsonValueKind.String 
                        ? unitElem.GetString() ?? "fahrenheit" 
                        : "fahrenheit";
                        
                    // Simple simulated result
                    return new 
                    { 
                        temperature = 72, 
                        conditions = "Sunny", 
                        location = location,
                        unit = unit
                    };
                });
                
            // Register a simple calculator tool
            server.AddTool(
                "calculate", 
                "Performs basic arithmetic calculations",
                (args, ctx) => 
                {
                    int a = args.TryGetValue("a", out var aElem) && aElem.ValueKind == JsonValueKind.Number 
                        ? aElem.GetInt32() 
                        : 0;
                        
                    int b = args.TryGetValue("b", out var bElem) && bElem.ValueKind == JsonValueKind.Number 
                        ? bElem.GetInt32() 
                        : 0;
                        
                    string operation = args.TryGetValue("operation", out var opElem) && opElem.ValueKind == JsonValueKind.String 
                        ? opElem.GetString() ?? "add" 
                        : "add";
                    
                    return operation switch
                    {
                        "add" => a + b,
                        "subtract" => a - b,
                        "multiply" => a * b,
                        "divide" => b != 0 ? a / b : 0,
                        _ => 0
                    };
                });
                
            // Register a simple greeting prompt
            server.AddPrompt(
                "greeting", 
                "Generates a customized greeting message",
                (args, ctx) => 
                {
                    string name = args.TryGetValue("name", out var nameElem) && nameElem.ValueKind == JsonValueKind.String 
                        ? nameElem.GetString() ?? "User" 
                        : "User";
                        
                    bool formal = args.TryGetValue("formal", out var formalElem) && formalElem.ValueKind == JsonValueKind.True;
                    
                    string greeting = formal ? "Greetings" : "Hey";
                    
                    var timeOfDay = DateTime.Now.Hour switch
                    {
                        >= 5 and < 12 => "morning",
                        >= 12 and < 17 => "afternoon",
                        _ => "evening"
                    };
                    
                    // Return a chat message in the format expected by MCP
                    return new List<ChatMessage> 
                    { 
                        new ChatMessage 
                        { 
                            Role = "system", 
                            Content = new ContentItem { Type = "text", Text = $"{greeting} {name}, good {timeOfDay}!" } 
                        } 
                    };
                });
                
            // Add a simple static resource
            server.AddResource(
                "file://example/readme", 
                "Example README", 
                "An example text resource",
                (ctx) => "# Example MCP Resource\n\nThis is a simple text resource served by the MCP server."
            );

            Console.Error.WriteLine("Server configured with:");
            Console.Error.WriteLine("- Tools: getWeather, calculate");
            Console.Error.WriteLine("- Prompts: greeting");
            Console.Error.WriteLine("- Resources: file://example/readme");
            Console.Error.WriteLine("Waiting for JSON-RPC messages on standard input...");
            
            // Create a StdioTransport for communication
            var transport = new StdioTransport();
            
            // Connect the server to the transport
            await server.ConnectAsync(transport);
            
            // Keep the application running
            await Task.Delay(-1);
        }
    }
}