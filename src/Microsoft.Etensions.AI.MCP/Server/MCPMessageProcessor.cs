using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.AI.MCP.Annotations;
using Microsoft.Extensions.AI.MCP.Initialization;
using Microsoft.Extensions.AI.MCP.Messages;
using Microsoft.Extensions.AI.MCP.Models;
using Microsoft.Extensions.AI.MCP.Server.Models;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.AI.MCP.Server
{
    /// <summary>
    /// Default implementation of the MCP message processor.
    /// </summary>
    public class MCPMessageProcessor : IMCPMessageProcessor
    {
        private readonly ILogger<MCPMessageProcessor> _logger;
        private readonly IMCPServerInitializer _serverInitializer;

        /// <summary>
        /// Initializes a new instance of <see cref="MCPMessageProcessor"/>.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="serverInitializer">The MCP server initializer.</param>
        public MCPMessageProcessor(ILogger<MCPMessageProcessor> logger, IMCPServerInitializer serverInitializer)
        {
            _logger = logger;
            _serverInitializer = serverInitializer;
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
            
            var result = await _serverInitializer.InitializeAsync(initRequest, cancellationToken);
            
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

        /// <summary>
        /// Handles a get_prompt request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The response.</returns>
        protected virtual async Task<object> HandleGetPromptRequestAsync(JsonRpcRequest<GetPromptParams> request, CancellationToken cancellationToken)
        {
            var promptName = request.Params.PromptName;
            var prompts = MCPRoutingExtensions.GetPrompts();
            
            if (!prompts.TryGetValue(promptName, out var promptDefinition))
            {
                return new JsonRpcResponse<object>
                {
                    Id = request.Id,
                    Error = new JsonRpcError
                    {
                        Code = -32602,
                        Message = $"Prompt not found: {promptName}"
                    }
                };
            }

            try
            {
                // Deserialize parameters from JsonElement
                Type parameterType = promptDefinition.Handler.Method.GetParameters()[0].ParameterType;
                object? parameters = null;
                
                if (request.Params.Parameters != null)
                {
                    var paramJson = JsonSerializer.Serialize(request.Params.Parameters);
                    parameters = JsonSerializer.Deserialize(paramJson, parameterType);
                }
                else
                {
                    // Create default instance
                    parameters = Activator.CreateInstance(parameterType);
                }
                
                // Invoke the handler
                var handlerType = promptDefinition.Handler.GetType();
                var invokeMethod = handlerType.GetMethod("Invoke");
                
                dynamic handler = promptDefinition.Handler;
                var resultTask = handler(parameters);
                var result = await resultTask;
                
                // Create proper response
                return new JsonRpcResponse<GetPromptResult>
                {
                    Id = request.Id,
                    Result = new GetPromptResult
                    {
                        Prompt = new Prompt
                        {
                            Messages = (result as PromptMessage[])?.ToList() ?? new List<PromptMessage>()
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing prompt {Name}", promptName);
                return new JsonRpcResponse<object>
                {
                    Id = request.Id,
                    Error = new JsonRpcError
                    {
                        Code = -32000,
                        Message = "Error executing prompt",
                        Data = ex.Message
                    }
                };
            }
        }

        /// <summary>
        /// Handles a call_tool request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The response.</returns>
        protected virtual async Task<object> HandleCallToolRequestAsync(JsonRpcRequest<CallToolRequest> request, CancellationToken cancellationToken)
        {
            var toolName = request.Params.Name;
            var tools = MCPRoutingExtensions.GetTools();
            
            if (!tools.TryGetValue(toolName, out var toolDefinition))
            {
                return new JsonRpcResponse<object>
                {
                    Id = request.Id,
                    Error = new JsonRpcError
                    {
                        Code = -32602,
                        Message = $"Tool not found: {toolName}"
                    }
                };
            }

            try
            {
                // Deserialize parameters from JsonElement
                Type parameterType = toolDefinition.Handler.Method.GetParameters()[0].ParameterType;
                object? parameters = null;
                
                if (request.Params.Arguments != null)
                {
                    var paramJson = JsonSerializer.Serialize(request.Params.Arguments);
                    parameters = JsonSerializer.Deserialize(paramJson, parameterType);
                }
                else
                {
                    // Create default instance
                    parameters = Activator.CreateInstance(parameterType);
                }
                
                // Invoke the handler
                if (toolDefinition.IsSync)
                {
                    // Synchronous tool
                    dynamic handler = toolDefinition.Handler;
                    var result = handler(parameters);
                    
                    // Create proper response
                    return new JsonRpcResponse<CallToolResult>
                    {
                        Id = request.Id,
                        Result = new CallToolResult
                        {
                            // Create a TextContent result from the tool result
                            Content = new List<object> { new TextContent { Text = result?.ToString() ?? string.Empty } }
                        }
                    };
                }
                else
                {
                    // Asynchronous tool
                    dynamic handler = toolDefinition.Handler;
                    var resultTask = handler(parameters);
                    var result = await resultTask;
                    
                    // Create proper response
                    return new JsonRpcResponse<CallToolResult>
                    {
                        Id = request.Id,
                        Result = new CallToolResult
                        {
                            // Create a TextContent result from the tool result
                            Content = new List<object> { new TextContent { Text = result?.ToString() ?? string.Empty } }
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing tool {Name}", toolName);
                return new JsonRpcResponse<object>
                {
                    Id = request.Id,
                    Error = new JsonRpcError
                    {
                        Code = -32000,
                        Message = "Error executing tool",
                        Data = ex.Message
                    }
                };
            }
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
                Id = message is JsonRpcRequest<object> request ? request.Id : default!,
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
                Id = message is JsonRpcRequest<object> request ? request.Id : default!,
                Error = error
            };
        }
    }
}