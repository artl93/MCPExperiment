using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.AI.MCP.Initialization;
using Microsoft.Extensions.AI.MCP.Messages;
using Microsoft.Extensions.AI.MCP.Models;
using Microsoft.Extensions.AI.MCP.Protocol.JsonRpc.Serialization;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.AI.MCP.Server
{
    /// <summary>
    /// Implementation of the JSON-RPC protocol handler.
    /// </summary>
    public class JsonRpcProtocolHandler : IMCPProtocolHandler
    {
        private readonly ILogger<JsonRpcProtocolHandler> _logger;
        private readonly JsonSerializerOptions _serializerOptions;

        /// <summary>
        /// Initializes a new instance of <see cref="JsonRpcProtocolHandler"/>.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public JsonRpcProtocolHandler(ILogger<JsonRpcProtocolHandler> logger)
        {
            _logger = logger;
            _serializerOptions = JsonRpcSerializer.CreateOptions();
        }

        /// <summary>
        /// Deserializes a message from a JSON string.
        /// </summary>
        /// <param name="json">The JSON string.</param>
        /// <returns>The deserialized message.</returns>
        public object DeserializeMessage(string json)
        {
            try
            {
                // First parse as a basic JsonRpcMessage to determine type
                var basicMessage = JsonSerializer.Deserialize<JsonDocument>(json, _serializerOptions);
                Debug.Assert(basicMessage != null);
                
                // Check if it's a request, response, or notification
                if (basicMessage.RootElement.TryGetProperty("method", out var methodElement))
                {
                    // It's a request or notification
                    string method = methodElement.GetString() ?? string.Empty;
                    
                    // Check if it has an id (request) or not (notification)
                    bool hasId = basicMessage.RootElement.TryGetProperty("id", out _);
                    
                    if (hasId)
                    {
                        // It's a request
                        return DeserializeRequest(json, method);
                    }
                    else
                    {
                        // It's a notification
                        return DeserializeNotification(json, method);
                    }
                }
                else if (basicMessage.RootElement.TryGetProperty("result", out _) || 
                         basicMessage.RootElement.TryGetProperty("error", out _))
                {
                    // It's a response
                    return DeserializeResponse(json);
                }
                
                throw new JsonException("Invalid JSON-RPC message: missing method, result, or error property");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deserializing JSON-RPC message: {Json}", json);
                throw;
            }
        }

        private object DeserializeRequest(string json, string method)
        {
            // Handle different request methods
            return method switch
            {
                "initialize" => JsonSerializer.Deserialize<JsonRpcRequest<InitializeRequestParams>>(json, _serializerOptions)!,
                "ping" => JsonSerializer.Deserialize<JsonRpcRequest<PingRequestParams>>(json, _serializerOptions)!,
                "get_prompt" => JsonSerializer.Deserialize<JsonRpcRequest<GetPromptParams>>(json, _serializerOptions)!,
                "call_tool" => JsonSerializer.Deserialize<JsonRpcRequest<CallToolRequest>>(json, _serializerOptions)!,
                _ => JsonSerializer.Deserialize<JsonRpcRequest<object>>(json, _serializerOptions)!
            };
        }

        private object DeserializeNotification(string json, string method)
        {
            // Handle different notification methods
            return method switch
            {
                "initialized" => JsonSerializer.Deserialize<JsonRpcNotification<object>>(json, _serializerOptions)!,
                _ => JsonSerializer.Deserialize<JsonRpcNotification<object>>(json, _serializerOptions)!
            };
        }

        private object DeserializeResponse(string json)
        {
            // For simplicity, we'll deserialize as a generic response
            return JsonSerializer.Deserialize<JsonRpcResponse<object>>(json, _serializerOptions)!;
        }

        /// <summary>
        /// Serializes a message to a JSON string.
        /// </summary>
        /// <param name="message">The message to serialize.</param>
        /// <returns>The serialized JSON string.</returns>
        public string SerializeMessage(object message)
        {
            try
            {
                return JsonSerializer.Serialize(message, message.GetType(), _serializerOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error serializing JSON-RPC message of type {Type}", message.GetType().Name);
                throw;
            }
        }

        /// <summary>
        /// Starts the protocol handler for stdin/stdout.
        /// </summary>
        /// <param name="processor">The message processor to handle incoming messages.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task StartStdioAsync(IMCPMessageProcessor processor, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting JSON-RPC over stdio");
            
            // Read from stdin and write to stdout
            using var reader = new StreamReader(Console.OpenStandardInput(), Encoding.UTF8);
            using var writer = new StreamWriter(Console.OpenStandardOutput(), Encoding.UTF8) { AutoFlush = true };
            
            // Buffer to hold message content
            var messageBuffer = new StringBuilder();
            var contentLength = -1;
            
            // Main processing loop
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogDebug("Waiting for message...");
                    // Read the first line to get the content length
                    var message = await reader.ReadLineAsync();
                    
                   
                    _logger.LogDebug("Received message: {Message}", message);
                    
                    // Process message
                    object? response = null;
                    try
                    {
                        var deserializedMessage = DeserializeMessage(message);
                        response = await processor.ProcessMessageAsync(deserializedMessage, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing message");
                        
                        // Create error response
                        var errorResponse = new JsonRpcResponse<object>
                        {
                            Id = default!,
                            Error = new JsonRpcError
                            {
                                Code = -32700,
                                Message = "Parse error",
                                Data = ex.Message
                            }
                        };
                        
                        response = errorResponse;
                    }
                    
                    // Send response if any
                    if (response != null)
                    {
                        var responseJson = SerializeMessage(response);
                        await WriteMessageAsync(writer, responseJson);
                    }
                    
                    // Reset content length for next message
                    contentLength = -1;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in stdio protocol handler");
                }
            }
        }

        private async Task WriteMessageAsync(TextWriter writer, string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            
            await writer.WriteAsync($"Content-Length: {bytes.Length}\r\n");
            await writer.WriteAsync("\r\n");
            await writer.WriteAsync(message);
            await writer.FlushAsync();
            
            _logger.LogDebug("Sent message: {Message}", message);
        }
    }
}