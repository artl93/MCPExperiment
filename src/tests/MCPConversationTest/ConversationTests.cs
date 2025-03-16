using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.MCP;
using Microsoft.MCP.Annotations;
using Microsoft.MCP.Capabilities;
using Microsoft.MCP.Initialization;
using Microsoft.MCP.Messages;
using Microsoft.MCP.Models;
using Microsoft.MCP.Protocol;
using Microsoft.MCP.Protocol.JsonRpc;
using Microsoft.MCP.Resources;
using Microsoft.MCP.Server;
using Microsoft.MCP.Server.Models;
using Xunit;
using Xunit.Abstractions;

namespace MCPConversationTest
{
    public class ConversationTests
    {
        private readonly ITestOutputHelper _output;

        public ConversationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task TestConversationFromJsonFile()
        {
            // Load the test conversation from the JSON file
            string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TestConversation.json");
            string jsonContent = await File.ReadAllTextAsync(jsonFilePath);
            
            var conversationEntries = JsonSerializer.Deserialize<JsonElement[]>(jsonContent);
            Assert.NotNull(conversationEntries);
            
            // Set up mock transport to capture messages
            var clientTransport = new MockTransport("Client");
            var serverTransport = new MockTransport("Server");
            
            // Connect the mock transports (messages sent from client go to server and vice versa)
            clientTransport.SetPeer(serverTransport);
            serverTransport.SetPeer(clientTransport);
            
            // Set up client and server
            var client = new MCPClient("test-client", "1.0.0");
            var server = new MCPServer("test-server", "1.0.0");
            
            // Configure server with a sample tool
            server.AddTool(
                "read_file", 
                "Read the complete contents of a file from the file system.",
                (args, ctx) => 
                {
                    // This is a mock implementation that will return the content from the test file
                    // regardless of the actual file path requested
                    if (args.TryGetValue("path", out var pathElement))
                    {
                        string path = pathElement.GetString() ?? string.Empty;
                        _output.WriteLine($"Mock read_file tool called with path: {path}");
                        
                        // Return mock content similar to what's in the test file
                        return new TextContent 
                        { 
                            Text = "# Define constants for the screen width and height\nSCREEN_WIDTH = 800\nSCREEN_HEIGHT = 600\n\n# Define a Player object by extending pygame.sprite.Sprite\n# The surface drawn on the screen is now an attribute of 'player'\nclass Player(pygame.sprite.Sprite):\n    def __init__(self):\n        super(Player, self).__init__()\n        self.surf = pygame.Surface((75, 25))\n        self.surf.fill((255, 255, 255))\n        self.rect = self.surf.get_rect()\n\n# Initalize pygame\npygame.init()" 
                        };
                    }
                    return new TextContent { Text = "Error: No path provided" };
                });
            
            // Start the client and server
            var serverTask = server.ConnectAsync(serverTransport);
            var clientTask = client.ConnectAsync(clientTransport);
            
            // Wait for the connections to be established
            await Task.WhenAll(serverTask, clientTask);
            _output.WriteLine("Client and server connected");
            
            // Simulate the conversation based on the JSON file
            for (int i = 0; i < conversationEntries.Length; i++)
            {
                var entry = conversationEntries[i];
                
                // Skip empty messages
                if (entry.ValueKind == JsonValueKind.Object && !entry.EnumerateObject().Any())
                {
                    _output.WriteLine($"Entry {i}: Skipping empty message");
                    continue;
                }
                
                if (entry.TryGetProperty("method", out var methodElement))
                {
                    string method = methodElement.GetString() ?? string.Empty;
                    _output.WriteLine($"Entry {i}: Client sending method {method}");
                    
                    // This is a client -> server message
                    if (method == "initialize")
                    {
                        // Initialization is already handled by the ConnectAsync method
                        _output.WriteLine("Client already initialized through ConnectAsync");
                    }
                    else if (method == "tools/list")
                    {
                        var tools = await client.ListToolsAsync();
                        _output.WriteLine($"Client received tool list with {tools.Count} tools");
                        Assert.Contains(tools, t => t.Name == "read_file");
                    }
                    else if (method == "tools/call" && entry.TryGetProperty("params", out var paramsElement))
                    {
                        string toolName = paramsElement.GetProperty("name").GetString() ?? string.Empty;
                        var arguments = new Dictionary<string, object>();
                        
                        if (paramsElement.TryGetProperty("arguments", out var argsElement) && 
                            argsElement.TryGetProperty("path", out var pathElement))
                        {
                            arguments["path"] = pathElement.GetString() ?? string.Empty;
                        }
                        
                        var result = await client.CallToolAsync(toolName, arguments);
                        _output.WriteLine($"Client called tool {toolName} and received result");
                        Assert.NotEmpty(result);
                        Assert.Contains(result, c => c is TextContent textContent && !string.IsNullOrEmpty(textContent.Text));
                    }
                    else if (method == "ping")
                    {
                        _output.WriteLine("Client sent ping (no response expected)");
                    }
                }
                else if (entry.TryGetProperty("content", out var contentElement))
                {
                    // This is a server -> client response
                    _output.WriteLine($"Entry {i}: Server response with content");
                }
            }
            
            _output.WriteLine("Test conversation completed successfully");
        }
    }

    // Mock transport implementation that captures messages and forwards them to a peer
    public class MockTransport : IMCPTransport
    {
        private readonly string _name;
        private readonly List<string> _messages = new List<string>();
        private Func<string, Task>? _messageHandler;
        private MockTransport? _peer;
        private bool _isRunning = false;
        
        public MockTransport(string name)
        {
            _name = name;
        }
        
        public void SetPeer(MockTransport peer)
        {
            _peer = peer;
        }
        
        public Task StartAsync(Func<string, Task> messageHandler)
        {
            _messageHandler = messageHandler;
            _isRunning = true;
            return Task.CompletedTask;
        }
        
        public Task StopAsync()
        {
            _isRunning = false;
            _messageHandler = null;
            return Task.CompletedTask;
        }
        
        public Task SendAsync(string message)
        {
            _messages.Add(message);
            
            // Forward the message to the peer if available
            if (_peer != null && _peer._messageHandler != null && _peer._isRunning)
            {
                return _peer._messageHandler(message);
            }
            
            return Task.CompletedTask;
        }
        
        public List<string> GetMessages()
        {
            return _messages;
        }
    }

}