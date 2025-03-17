# Model Context Protocol (MCP) Server

This package provides an implementation of the Model Context Protocol (MCP) server for ASP.NET Core applications.

## Getting Started

### Installation

Add the MCP package to your project:

```bash
dotnet add package Microsoft.Extensions.MCP
```

### Using MCP with ASP.NET Core

Register the MCP server in your Program.cs or Startup.cs:

```csharp
using Microsoft.Extensions.AI.MCP;
using Microsoft.Extensions.AI.MCP.Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddMCP(options =>
{
    // Configure MCP server options
    options.EnableStdioProtocol = false; // Only enable for console applications
    options.ProtocolVersion = "1.0"; // Set the protocol version
    
    // Configure server capabilities
    options.ServerCapabilities.Prompts = new ServerPromptsCapabilities
    {
        // Configure prompt capabilities
    };
    
    options.ServerCapabilities.Tools = new ServerToolsCapabilities
    {
        // Configure tool capabilities
    };
    
    // Add custom experimental capabilities
    options.ExperimentalCapabilities["customFeature"] = true;
});

var app = builder.Build();

// Add MCP middleware to the request pipeline
app.UseMCP();

app.Run();
```

### Implementing Custom Message Handling

To customize the handling of MCP messages, implement your own `IMCPMessageProcessor`:

```csharp
public class CustomMessageProcessor : MCPMessageProcessor
{
    public CustomMessageProcessor(ILogger<CustomMessageProcessor> logger, IMCPServer server)
        : base(logger, server)
    {
    }
    
    // Override methods to customize handling of specific requests
    protected override async Task<object> HandleGetPromptRequestAsync(
        JsonRpcRequest<GetPromptParams> request, 
        CancellationToken cancellationToken)
    {
        // Custom implementation
        var result = new GetPromptResult
        {
            // Populate result
        };
        
        return new JsonRpcResponse<GetPromptResult>
        {
            Id = request.Id,
            Result = result
        };
    }
}
```

Register your custom processor in the service collection:

```csharp
builder.Services.AddTransient<IMCPMessageProcessor, CustomMessageProcessor>();
```

### Using MCP with Console Applications

For console applications that need to use stdin/stdout for communication:

```csharp
using Microsoft.Extensions.AI.MCP;
using Microsoft.Extensions.AI.MCP.Server;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddMCP(options =>
        {
            options.EnableStdioProtocol = true;
        });
    })
    .Build();

// Run the MCP server using stdin/stdout
await host.RunMCPConsoleAsync();
```

## Protocol Details

The MCP server implements the Model Context Protocol, which is a JSON-RPC 2.0 based protocol for communication between clients and AI model servers. The protocol supports:

- Initialization handshake
- Prompt retrieval
- Tool calling
- Various content types (text, images, audio)
- Progress notifications

For more details on the protocol, see the reference documentation.