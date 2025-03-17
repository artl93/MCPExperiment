using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using MCPExperiment;
using MCPExperiment.Annotations;
using MCPExperiment.Models;
using MCPExperiment.Server;
using MCPExperiment.TestApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace MCPExperiment.TestApp.HttpMCP
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await RunHttpServer();
        }

        private static async Task RunHttpServer()
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
                Console.WriteLine($"Request headers: {context.Request.Headers}");
                if (context.Request.Method == "POST")
                {
                    using var reader = new System.IO.StreamReader(context.Request.Body);
                    var body = await reader.ReadToEndAsync();
                    Console.WriteLine($"Request body: {body}");
                    context.Request.Body.Position = 0; // Reset the stream position for further processing
                }
                if (context.Request.Method == "GET")
                {
                    Console.WriteLine($"Query string: {context.Request.QueryString}");
                }
                // Call the next middleware in the pipeline
                await next();
            });
            
            // Map tools and prompts BEFORE configuring the HTTP pipeline
            // This ensures tools are registered before SSE connections are established
            app.MapTool<WeatherRequest, WeatherResponse>(
                "getWeather",
                "Gets the weather for a specific location",
                async (request) => await SharedImplementations.GetWeatherAsync(request));
            
            app.MapSyncTool<CalculatorRequest, int>(
                "calculate",
                "Performs a calculation",
                (request) => SharedImplementations.CalculateResult(request));
            
            app.MapPrompt<GreetingRequest, PromptMessage[]>(
                "greeting",
                "Generates a greeting message",
                "Messaging",
                async (request) => await SharedImplementations.GenerateGreetingAsync(request));
                
            // Get the server instance
            var server = app.Services.GetRequiredService<IMCPServer>();
            Console.WriteLine("Obtained MCP Server instance from DI container");
            
            // IMPORTANT: Initialize the server BEFORE configuring the HTTP pipeline
            // to ensure tools are available when SSE connections are established
            try 
            {
                var initRequest = new MCPExperiment.Initialization.InitializeRequest
                {
                    Params = new MCPExperiment.Initialization.InitializeRequestParams
                    {
                        ProtocolVersion = "2024-11-05",
                        Capabilities = new MCPExperiment.Capabilities.ClientCapabilities(),
                        ClientInfo = new MCPExperiment.Capabilities.Implementation
                        {
                            Name = "HttpMCPApp",
                            Version = "1.0"
                        }
                    }
                };
                var result = server.InitializeAsync(initRequest).GetAwaiter().GetResult();
                Console.WriteLine($"MCP Server manually initialized with {result.Capabilities.Tools?.AvailableTools?.Count ?? 0} tools and {result.Capabilities.Prompts?.AvailablePrompts?.Count ?? 0} prompts");
                
                // Send an explicit tool list notification to ensure SSE clients get tool information
                server.SendToolListChangedNotification();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize MCP Server: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            
            // Configure the HTTP pipeline with full spec support AFTER tools are registered and server is initialized
            app.UseMCPWithSSE();
            
            // Add MCP test/debug endpoint
            app.MapGet("/", () => "HTTP MCP Server is running. Try /debug/mcp-status for more information.");
            
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
        .event.toolsChanged { background-color: #cce5ff; }
        .event.test { background-color: #fff3cd; }
        .event.error { background-color: #f8d7da; }
        .timestamp { color: #666; font-size: 0.8em; }
        h1 { color: #333; }
        button { padding: 8px 16px; background: #007bff; color: white; border: none; cursor: pointer; }
        button:hover { background: #0069d9; }
        #tools { border: 1px solid #ccc; padding: 10px; max-height: 200px; overflow-y: auto; margin-bottom: 20px; }
    </style>
</head>
<body>
    <h1>MCP Server-Sent Events Demo</h1>
    <p>This page demonstrates Server-Sent Events from the MCP server.</p>
    
    <button id=""connect"">Connect to SSE</button>
    <button id=""call-tool"">Call Weather Tool</button>
    <button id=""test-sse"">Test SSE Event</button>
    <button id=""disconnect"">Disconnect</button>
    
    <h2>Available Tools:</h2>
    <div id=""tools"">No tools information received yet</div>
    
    <h2>Events:</h2>
    <div id=""events""></div>
    
    <script>
        let eventSource = null;
        let toolsList = [];
        
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
            
            // If this is a tool list notification, update tools display
            if (type === 'toolsChanged' && data) {
                // Handle both possible formats
                const tools = data.params?.availableTools || data.params?.AvailableTools || [];
                if (tools && tools.length > 0) {
                    updateToolsDisplay(tools);
                } else {
                    console.warn('No tools found in notification data:', data);
                }
            }
        }
        
        function updateToolsDisplay(tools) {
            toolsList = tools;
            const toolsDiv = document.getElementById('tools');
            toolsDiv.innerHTML = '';
            
            if (!tools || tools.length === 0) {
                toolsDiv.textContent = 'No tools available';
                return;
            }
            
            console.log('Tools to display:', tools);
            
            const toolsListElement = document.createElement('ul');
            tools.forEach(tool => {
                const toolItem = document.createElement('li');
                // Handle both camelCase and PascalCase property names
                const name = tool.name || tool.Name || 'Unknown tool';
                const description = tool.description || tool.Description || 'No description';
                
                toolItem.innerHTML = `<strong>${name}</strong>: ${description}`;
                toolsListElement.appendChild(toolItem);
            });
            
            toolsDiv.appendChild(toolsListElement);
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
                
                // Listen for tool list notifications
                eventSource.addEventListener('notifications/tools/listChanged', (event) => {
                    console.log('Raw event data:', event.data);
                    try {
                        const data = JSON.parse(event.data);
                        addEvent('toolsChanged', data);
                        console.log('Tools list updated:', data);
                        
                        // Add a direct message to make debugging easier
                        const eventsDiv = document.getElementById('events');
                        const debugDiv = document.createElement('div');
                        debugDiv.style.color = 'blue';
                        debugDiv.textContent = `Raw data received: ${event.data}`;
                        eventsDiv.appendChild(debugDiv);
                    } catch (err) {
                        console.error('Error parsing event data:', err);
                        addEvent('error', `Failed to parse event data: ${err.message}\nRaw data: ${event.data}`);
                    }
                });
                
                // Listen for test events
                eventSource.addEventListener('test-event', (event) => {
                    console.log('Test event received:', event.data);
                    try {
                        const data = JSON.parse(event.data);
                        addEvent('test', data);
                    } catch (err) {
                        console.error('Error parsing test event data:', err);
                        addEvent('error', `Failed to parse test event: ${err.message}`);
                    }
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
        
        document.getElementById('test-sse').addEventListener('click', async () => {
            try {
                // Call the test SSE endpoint
                const response = await fetch('/test-sse');
                const result = await response.json();
                console.log('Test SSE response:', result);
                addEvent('normal', 'Triggered test SSE event');
            } catch (err) {
                addEvent('normal', 'Test SSE failed: ' + err.message);
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
            
            Console.WriteLine("HTTP MCP Server is running on http://localhost:5000");
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
            
            // Add an endpoint to send a tool list changed notification
            app.MapGet("/send-tools-notification", () => {
                server.SendToolListChangedNotification();
                return new { 
                    success = true, 
                    message = "Tool list notification sent",
                    toolCount = MCPRoutingExtensions.GetTools().Count
                };
            });
            
            // Add a test endpoint to try SSE with a simple event
            app.MapGet("/test-sse", (Microsoft.Extensions.Logging.ILogger<Program> logger) => {
                var connectionManager = app.Services.GetRequiredService<MCPExperiment.Server.SSE.SSEConnectionManager>();
                
                // Send a simple test event
                Task.Run(async () => {
                    try {
                        var testData = @"{""test"":true, ""message"":""This is a test event"", ""timestamp"":""" + DateTime.Now.ToString() + @"""}";
                        logger.LogInformation("Sending test SSE event to {Count} clients", connectionManager.ConnectionCount);
                        await connectionManager.SendEventToAllAsync("test-event", testData);
                        logger.LogInformation("Test event sent successfully");
                    }
                    catch (Exception ex) {
                        logger.LogError(ex, "Failed to send test SSE event");
                    }
                });
                
                return new { success = true, message = "Test SSE event sent" };
            });
            
            // Start the server
            await app.RunAsync();
        }
    }
}