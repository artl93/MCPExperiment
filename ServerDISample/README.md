# MCP Server Sample Application

This is a sample application that demonstrates how to use the Model Context Protocol (MCP) server implementation with ASP.NET Core.

## Features

- Implements both HTTP and stdio-based MCP servers
- Demonstrates tool and prompt registration using:
  - Direct method mapping
  - Attribute-based mapping
- Shows how to handle request/response cycle for tools and prompts
- Integrates with ASP.NET Core dependency injection

## Running the Sample

### HTTP Server

```
dotnet run
```

The server will start on http://localhost:5000 by default. You can send MCP requests to the `/mcp` endpoint.

### Stdio Server

```
dotnet run -- --stdio
```

This will start an MCP server that communicates over standard input/output using the JSON-RPC protocol.

## Testing with Example JSON Files

You can test the server using the provided example JSON files:

1. First, initialize the server:

```
cat InitializeRequestExample.json | nc localhost 5000/mcp
```

2. Then you can call tools or get prompts using similar JSON requests

## Available Tools and Prompts

### Tools

- `getWeather` - Gets weather information for a location
- `calculate` - Performs arithmetic calculations

### Prompts

- `greeting` - Generates a greeting message

## Implementation Details

This sample shows how to:

1. Register MCP services with dependency injection
2. Configure MCP middleware for HTTP scenarios
3. Configure stdio protocol for CLI scenarios
4. Define tools and prompts with parameters
5. Map tools and prompts to handler methods
6. Process MCP JSON-RPC messages