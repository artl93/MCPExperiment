using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MCPExperiment.Server
{
    /// <summary>
    /// ASP.NET Core middleware for the MCP server.
    /// </summary>
    public class MCPMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<MCPMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="MCPMiddleware"/>.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="logger">The logger.</param>
        public MCPMiddleware(RequestDelegate next, ILogger<MCPMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Processes an HTTP request.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="mcpServer">The MCP server.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context, IMCPServer mcpServer)
        {
            if (context.Request.Path.StartsWithSegments("/mcp", StringComparison.OrdinalIgnoreCase))
            {
                // Process MCP request
                await HandleMCPRequestAsync(context, mcpServer);
                return;
            }
            
            // Not an MCP request, continue with the pipeline
            await _next(context);
        }

        private async Task HandleMCPRequestAsync(HttpContext context, IMCPServer mcpServer)
        {
            string requestBody;
            
            // Read the request body
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            
            _logger.LogDebug("Received MCP request: {RequestBody}", requestBody);
            
            try
            {
                // Process the message
                string? responseJson = await mcpServer.ProcessMessageAsync(requestBody, context.RequestAborted);
                
                // Send the response
                if (responseJson != null)
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync(responseJson);
                }
                else
                {
                    // No response needed (e.g. notification)
                    context.Response.StatusCode = 204; // No content
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing MCP request");
                
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;
                
                var error = new
                {
                    jsonrpc = Constants.JsonRpcVersion,
                    id = (object)null!,
                    error = new
                    {
                        code = -32000,
                        message = "Internal server error",
                        data = ex.Message
                    }
                };
                
                await context.Response.WriteAsync(JsonSerializer.Serialize(error));
            }
        }
    }
}