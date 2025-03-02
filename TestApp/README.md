# Model Context Protocol (MCP) Test Applications

This directory contains two example implementations of the MCP protocol:

## 1. HttpMCP

A web server implementation that demonstrates the MCP protocol over HTTP with Server-Sent Events (SSE).

### Features:
- JSON-RPC endpoint at `/mcp` for all MCP requests
- SSE endpoint at `/sse` for real-time notifications
- Interactive demo page at `/sse-demo`
- Test endpoint to send manual notifications at `/send-notification`
- Debugging endpoint at `/debug/mcp-status`

### Usage:
```
cd HttpMCP
dotnet run
```

The server will be available at `http://localhost:5000`.

## 2. StdioMCP

A standard input/output (STDIO) implementation of the MCP protocol for CLI tools.

### Features:
- Reads JSON-RPC requests from stdin
- Writes JSON-RPC responses to stdout
- Supports the same tools and prompts as the HTTP version

### Usage:
```
cd StdioMCP
dotnet run
```

Type valid JSON-RPC requests into the console to interact with the server.

## Shared Functionality

Both implementations support:
- Weather tool (`getWeather`)
- Calculator tool (`calculate`)
- Greeting prompt (`greeting`)
- Other example tools with attributes

## Protocol Version

Both implementations use the MCP protocol version `2024-11-05`.