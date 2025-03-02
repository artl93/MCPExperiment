using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MCP
{
    /// <summary>Context passed into server handlers for extra operations and logging.</summary>
    public class McpContext
    {
        private readonly McpServer? _server;
        private readonly McpClient? _client;
        private readonly ILogger? _logger;
        private readonly string? _requestId;
        internal McpContext(McpServer? server, McpClient? client, ILogger? logger, string? requestId = null)
        {
            _server = server;
            _client = client;
            _logger = logger;
            _requestId = requestId;
        }
        /// <summary>Log an informational message (visible on the host side if integrated, or console).</summary>
        public void Info(string message)
        {
            _logger?.LogInformation("MCP: {Message}", message);
            // (In a full implementation, could also forward to client via a custom notification if desired.)
        }
        /// <summary>Report progress of a long-running server operation (sends a progress notification to client).</summary>
        public async Task ReportProgress(int current, int total)
        {
            if (_server != null && _requestId != null)
            {
                // We send a "progress" notification with current/total values
                var progress = new { current = current, total = total };
                await _server.SendNotificationAsync("progress", progress);
            }
        }
        /// <summary>From a server handler, read a resource via the client (host). Useful to fetch host-provided data.</summary>
        public async Task<(byte[]? data, string? mimeType)> ReadResourceAsync(string uri)
        {
            if (_client == null)
                throw new InvalidOperationException("ReadResourceAsync is only available in server context.");
            return await _client.ReadResourceAsync(uri);
        }
        /// <summary>From a server handler, request an LLM completion via the client's model (sampling API).</summary>
        public async Task<ChatMessage?> RequestCompletionAsync(List<ChatMessage> messages, string? systemPrompt = null)
        {
            if (_server == null)
                throw new InvalidOperationException("RequestCompletionAsync can only be called from server context.");
            try
            {
                var param = new { messages = messages, systemPrompt = systemPrompt };
                JsonElement result = await _server.SendRequestAsync("sampling/createMessage", param);
                // Expect a result with a 'message' field containing the assistant's reply
                if (result.ValueKind == JsonValueKind.Object && result.TryGetProperty("message", out var msgElem))
                {
                    return JsonSerializer.Deserialize<ChatMessage>(msgElem.GetRawText());
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Sampling request failed or not supported by client");
            }
            return null;
        }
    }
}