using System.Threading;
using System.Threading.Tasks;
using MCPExperiment.Initialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MCPExperiment.Server
{
    /// <summary>
    /// Bootstrap implementation of IMCPServerInitializer used to break circular dependency.
    /// This class provides minimum functionality until the real server is initialized.
    /// </summary>
    public class MCPServerBootstrap : IMCPServerInitializer
    {
        private readonly ILogger<MCPServerBootstrap> _logger;
        private readonly MCPServerOptions _options;
        private bool _initialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="MCPServerBootstrap"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The server options.</param>
        public MCPServerBootstrap(
            ILogger<MCPServerBootstrap> logger,
            IOptions<MCPServerOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        /// <summary>
        /// Initializes the server with the specified client capabilities.
        /// The bootstrap implementation just logs the request and returns a basic result.
        /// </summary>
        /// <param name="request">The initialize request from the client.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The initialization result.</returns>
        public Task<InitializeResult> InitializeAsync(InitializeRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Bootstrap server initializer called - this should only happen during DI resolution");
            
            var result = new InitializeResult
            {
                ProtocolVersion = _options.ProtocolVersion,
                // ServerCapabilities = _options.ServerCapabilities,
                ServerInfo = new Capabilities.Implementation
                {
                    Name = "Bootstrap MCP Server",
                    Version = "1.0"
                }
            };

            _initialized = true;
            
            return Task.FromResult(result);
        }

        /// <summary>
        /// Sends a notification to the client.
        /// The bootstrap implementation just logs the notification but doesn't send it.
        /// </summary>
        /// <param name="notification">The notification to send.</param>
        public void SendNotification(object notification)
        {
            _logger.LogWarning("Bootstrap server initializer cannot send notifications");
        }
        
        /// <summary>
        /// Sends a demo notification (bootstrap implementation does nothing).
        /// </summary>
        /// <param name="message">The message to include in the notification.</param>
        public void SendDemoNotification(string message)
        {
            _logger.LogWarning("Bootstrap server initializer cannot send demo notifications");
        }
    }
}