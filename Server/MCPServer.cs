using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.AI.MCP.Capabilities;
using Microsoft.Extensions.AI.MCP.Initialization;
using Microsoft.Extensions.AI.MCP.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.AI.MCP.Server
{
    /// <summary>
    /// Default implementation of the MCP server.
    /// </summary>
    public class MCPServer : IMCPServer
    {
        private readonly IMCPProtocolHandler _protocolHandler;
        private readonly IMCPMessageProcessor _messageProcessor;
        private readonly MCPServerOptions _options;
        private readonly ILogger<MCPServer> _logger;
        private bool _isInitialized;

        /// <summary>
        /// Event raised when the server needs to send a notification to the client.
        /// </summary>
        public event EventHandler<ServerNotificationEventArgs>? NotificationReady;

        /// <summary>
        /// Initializes a new instance of <see cref="MCPServer"/>.
        /// </summary>
        /// <param name="protocolHandler">The protocol handler.</param>
        /// <param name="messageProcessor">The message processor.</param>
        /// <param name="options">The server options.</param>
        /// <param name="logger">The logger.</param>
        public MCPServer(
            IMCPProtocolHandler protocolHandler,
            IMCPMessageProcessor messageProcessor,
            IOptions<MCPServerOptions> options,
            ILogger<MCPServer> logger)
        {
            _protocolHandler = protocolHandler;
            _messageProcessor = messageProcessor;
            _options = options.Value;
            _logger = logger;
        }

        /// <summary>
        /// Initializes the server with the specified client capabilities.
        /// </summary>
        /// <param name="request">The initialize request from the client.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The initialization result.</returns>
        public Task<InitializeResult> InitializeAsync(InitializeRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Initializing MCP server with protocol version {Version}", request.Params.ProtocolVersion);
            
            // Merge experimental capabilities if any
            if (_options.ExperimentalCapabilities.Count > 0 && _options.ServerCapabilities.Experimental != null)
            {
                foreach (var capability in _options.ExperimentalCapabilities)
                {
                    _options.ServerCapabilities.Experimental[capability.Key] = capability.Value;
                }
            }

            var result = new InitializeResult
            {
                ProtocolVersion = _options.ProtocolVersion,
                ServerCapabilities = _options.ServerCapabilities
            };

            _isInitialized = true;
            
            return Task.FromResult(result);
        }

        /// <summary>
        /// Processes a raw JSON-RPC message from the client.
        /// </summary>
        /// <param name="message">The raw JSON-RPC message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The response message if any.</returns>
        public async Task<string?> ProcessMessageAsync(string message, CancellationToken cancellationToken = default)
        {
            try
            {
                var deserializedMessage = _protocolHandler.DeserializeMessage(message);
                
                // Handle initialization request specially
                if (deserializedMessage is JsonRpcRequest<InitializeRequestParams> initRequest)
                {
                    var initParams = initRequest.Params;
                    var initializeRequest = new InitializeRequest
                    {
                        Params = initParams
                    };
                    
                    var result = await InitializeAsync(initializeRequest, cancellationToken);
                    
                    // Create JSON-RPC response
                    var response = new JsonRpcResponse<InitializeResult>
                    {
                        Id = ((JsonRpcRequest<InitializeRequestParams>)deserializedMessage).Id,
                        Result = result
                    };
                    
                    return _protocolHandler.SerializeMessage(response);
                }
                
                // For all other messages, require initialization first
                if (!_isInitialized)
                {
                    var error = new JsonRpcError
                    {
                        Code = -32002,
                        Message = "Server not initialized"
                    };
                    
                    var errorResponse = new JsonRpcResponse<object>
                    {
                        Id = deserializedMessage is JsonRpcRequest<object> req ? req.Id : null,
                        Error = error
                    };
                    
                    return _protocolHandler.SerializeMessage(errorResponse);
                }
                
                // Process the message
                var responseObj = await _messageProcessor.ProcessMessageAsync(deserializedMessage, cancellationToken);
                
                // For notifications, there's no response
                if (responseObj == null)
                {
                    return null;
                }
                
                return _protocolHandler.SerializeMessage(responseObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message: {Message}", message);
                
                var error = new JsonRpcError
                {
                    Code = -32000,
                    Message = "Internal server error",
                    Data = ex.Message
                };
                
                var errorResponse = new JsonRpcResponse<object>
                {
                    Id = null, // We may not be able to parse the ID
                    Error = error
                };
                
                return _protocolHandler.SerializeMessage(errorResponse);
            }
        }

        /// <summary>
        /// Starts the stdio protocol handler if enabled.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task StartStdioAsync(CancellationToken cancellationToken = default)
        {
            if (!_options.EnableStdioProtocol)
            {
                _logger.LogInformation("Stdio protocol is disabled");
                return Task.CompletedTask;
            }
            
            _logger.LogInformation("Starting stdio protocol handler");
            return _protocolHandler.StartStdioAsync(_messageProcessor, cancellationToken);
        }

        /// <summary>
        /// Sends a notification to the client.
        /// </summary>
        /// <param name="notification">The notification to send.</param>
        public void SendNotification(object notification)
        {
            var json = _protocolHandler.SerializeMessage(notification);
            NotificationReady?.Invoke(this, new ServerNotificationEventArgs(json));
        }
    }
}