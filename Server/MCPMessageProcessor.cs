using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.AI.MCP.Initialization;
using Microsoft.Extensions.AI.MCP.Messages;
using Microsoft.Extensions.AI.MCP.Models;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.AI.MCP.Server
{
    /// <summary>
    /// Default implementation of the MCP message processor.
    /// </summary>
    public class MCPMessageProcessor : IMCPMessageProcessor
    {
        private readonly ILogger<MCPMessageProcessor> _logger;
        private readonly IMCPServer _server;

        /// <summary>
        /// Initializes a new instance of <see cref="MCPMessageProcessor"/>.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="server">The MCP server.</param>
        public MCPMessageProcessor(ILogger<MCPMessageProcessor> logger, IMCPServer server)
        {
            _logger = logger;
            _server = server;
        }

        /// <summary>
        /// Processes a deserialized message.
        /// </summary>
        /// <param name="message">The deserialized message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The response message, or null if no response is needed.</returns>
        public async Task<object?> ProcessMessageAsync(object message, CancellationToken cancellationToken = default)
        {
            try
            {
                // Handle different message types
                switch (message)
                {
                    case JsonRpcRequest<InitializeRequestParams> initRequest:
                        return await HandleInitializeRequestAsync(initRequest, cancellationToken);
                    
                    case JsonRpcRequest<PingRequestParams> pingRequest:
                        return HandlePingRequest(pingRequest);
                    
                    case JsonRpcNotification<object> notification:
                        return await HandleNotificationAsync(notification, cancellationToken);
                    
                    case JsonRpcRequest<GetPromptParams> getPromptRequest:
                        return await HandleGetPromptRequestAsync(getPromptRequest, cancellationToken);
                    
                    case JsonRpcRequest<CallToolRequest> callToolRequest:
                        return await HandleCallToolRequestAsync(callToolRequest, cancellationToken);
                    
                    default:
                        _logger.LogWarning("Unsupported message type: {Type}", message.GetType().Name);
                        return CreateMethodNotFoundError(message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message: {Type}", message.GetType().Name);
                return CreateInternalError(message, ex.Message);
            }
        }

        private async Task<object> HandleInitializeRequestAsync(JsonRpcRequest<InitializeRequestParams> request, CancellationToken cancellationToken)
        {
            var initRequest = new InitializeRequest
            {
                Params = request.Params
            };
            
            var result = await _server.InitializeAsync(initRequest, cancellationToken);
            
            return new JsonRpcResponse<InitializeResult>
            {
                Id = request.Id,
                Result = result
            };
        }

        private object HandlePingRequest(JsonRpcRequest<PingRequestParams> request)
        {
            return new JsonRpcResponse<EmptyResult>
            {
                Id = request.Id,
                Result = new EmptyResult()
            };
        }

        private Task<object?> HandleNotificationAsync(JsonRpcNotification<object> notification, CancellationToken cancellationToken)
        {
            // Handle notification based on method
            if (notification.Method == "initialized")
            {
                _logger.LogInformation("Client initialization complete");
                // No response for notifications
                return Task.FromResult<object?>(null);
            }
            
            _logger.LogWarning("Unsupported notification method: {Method}", notification.Method);
            return Task.FromResult<object?>(null);
        }

        private Task<object> HandleGetPromptRequestAsync(JsonRpcRequest<GetPromptParams> request, CancellationToken cancellationToken)
        {
            // This would be implemented by classes that extend this base processor
            var error = new JsonRpcError
            {
                Code = -32601,
                Message = "Method not implemented: get_prompt"
            };
            
            return Task.FromResult<object>(new JsonRpcResponse<object>
            {
                Id = request.Id,
                Error = error
            });
        }

        private Task<object> HandleCallToolRequestAsync(JsonRpcRequest<CallToolRequest> request, CancellationToken cancellationToken)
        {
            // This would be implemented by classes that extend this base processor
            var error = new JsonRpcError
            {
                Code = -32601,
                Message = "Method not implemented: call_tool"
            };
            
            return Task.FromResult<object>(new JsonRpcResponse<object>
            {
                Id = request.Id,
                Error = error
            });
        }

        private object CreateMethodNotFoundError(object message)
        {
            var error = new JsonRpcError
            {
                Code = -32601,
                Message = "Method not found"
            };
            
            return new JsonRpcResponse<object>
            {
                Id = message is JsonRpcRequest<object> request ? request.Id : null,
                Error = error
            };
        }

        private object CreateInternalError(object message, string details)
        {
            var error = new JsonRpcError
            {
                Code = -32000,
                Message = "Internal server error",
                Data = details
            };
            
            return new JsonRpcResponse<object>
            {
                Id = message is JsonRpcRequest<object> request ? request.Id : null,
                Error = error
            };
        }
    }
}