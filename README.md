# MCP Experiment (WIP)

A .NET implementation of the Model Context Protocol (MCP) for client-server communication with AI models. # Contributing to MCP Experiment. 

*This was an experiment using Claude Code to generate an SDK from schema, directing it toward emulating minimal web apis, building validation tests and comparing other AI-generated implementations. It is here for sharing purposes only and not intended for general use.*

If you find it useful, go for it. 

## Overview

MCP provides a standardized communication protocol between client applications and AI model services. It's built on JSON-RPC 2.0 and supports various content types including text, images, and audio.

## Features

- **Multiple Transport Methods**: Support for HTTP/REST APIs and stdio for CLI applications
- **Real-time Communication**: Server-Sent Events (SSE) for progress notifications 
- **Content Support**: Handle text, image, and audio content in both requests and responses
- **Tool Integration**: Register and invoke AI model tools with attribute-based mapping
- **Prompt Management**: Define and retrieve prompts with parameter support
- **Resource Handling**: Template-based resource management

## Getting Started

### Basic Usage

#### Server Implementation

```csharp
// Configure and run an MCP server with ASP.NET Core
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMcpServer();
var app = builder.Build();
app.UseMcp();
app.Run();
```

#### Client Implementation

```csharp
// Create an MCP client and make requests
var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("https://localhost:5000");
var mcpClient = new McpClient(httpClient);
await mcpClient.InitializeAsync();

// Get a prompt
var result = await mcpClient.GetPromptAsync("myPrompt", new { parameter = "value" });
```

## Examples

Check the `src/tests` directory for example applications:

- `HttpMCP`: Web server implementation 
- `StdioMCP`: CLI-based implementation
- `ServerDISample`: Shows dependency injection with MCP

## Documentation

For detailed documentation, please see the XML comments in the source code or generate documentation with a tool like DocFX.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct and the process for submitting pull requests.
