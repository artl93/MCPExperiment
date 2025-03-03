using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.AI.MCP.Messages;

namespace MCP
{
    /// <summary>Represents an MCP client (runs in host, connects to a server instance).</summary>
    public class McpClient
    {
        private readonly ImplementationInfo _clientInfo;
        private readonly ClientCapabilities _capabilities = new ClientCapabilities();
        private IMcpTransport? _transport;
        private readonly ILogger? _logger;
        private bool _initialized = false;
        private ImplementationInfo? _serverInfo;
        private ServerCapabilities? _serverCaps;

        // Optional handler for server-initiated sampling requests:
        public Func<List<ChatMessage>, string?, Task<ChatMessage>>? OnSamplingRequest;

        private List<Root>? _initialRoots;

        public McpClient(string name, string version, ILogger? logger = null)
        {
            _clientInfo = new ImplementationInfo { Name = name, Version = version };
            _logger = logger;
        }
        /// <summary>If the client supports servicing sampling requests, call this before Connect.</summary>
        public void SetSamplingSupport(bool support)
        {
            _capabilities.SupportsSampling = support;
        }
        /// <summary>Set initial root(s) to send to the server for context limiting (optional).</summary>
        public void SetRoots(List<Root> roots)
        {
            _initialRoots = roots;
            _capabilities.SupportsRoots = true;
        }

        /// <summary>Connect to a server via a given transport. Performs handshake automatically.</summary>
        public async Task ConnectAsync(IMcpTransport transport)
        {
            _transport = transport;
            _logger?.LogDebug("MCP client connecting (name={0}, version={1})", _clientInfo.Name, _clientInfo.Version);
            // Start reading messages in background
            await _transport.StartAsync(async msg =>
            {
                try
                {
                    await ProcessMessageAsync(msg);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error processing MCP client message");
                }
            });
            // Perform handshake: send initialize and wait for response
            var initParams = new InitializeParams { Client = _clientInfo, Capabilities = _capabilities, ProtocolVersion = "1.0", Roots = _initialRoots };
            JsonElement initResultElem;
            try
            {
                initResultElem = await SendRequestAsync("initialize", initParams);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to initialize MCP connection", ex);
            }
            var initResult = JsonSerializer.Deserialize<InitializeResult>(initResultElem.GetRawText());
            _serverInfo = initResult?.Server;
            _serverCaps = initResult?.Capabilities;
            // Acknowledge initialization
            await SendNotificationAsync("initialized", null);
            _initialized = true;
            _logger?.LogInformation("MCP client connected (server: {0} v{1})", _serverInfo?.Name, _serverInfo?.Version);
        }

        private async Task ProcessMessageAsync(string jsonMessage)
        {
            using JsonDocument doc = JsonDocument.Parse(jsonMessage);
            JsonElement root = doc.RootElement;
            if (root.TryGetProperty("method", out var methodElem))
            {
                string method = methodElem.GetString()!;
                if (root.TryGetProperty("id", out var idElem))
                {
                    // server -> client request
                    string idRaw = idElem.GetRawText();
                    JsonElement paramsElem = root.GetProperty("params");
                    await HandleServerRequestAsync(idRaw, method, paramsElem);
                }
                else
                {
                    // server -> client notification
                    JsonElement paramsElem = root.TryGetProperty("params", out var pElem) ? pElem : default;
                    await HandleServerNotificationAsync(method, paramsElem);
                }
            }
            else if (root.TryGetProperty("id", out var respIdElem))
            {
                // response to a request initiated by client
                string respIdRaw = respIdElem.GetRawText();
                if (_pendingRequests.TryGetValue(respIdRaw, out var tcs))
                {
                    _pendingRequests.Remove(respIdRaw);
                    if (root.TryGetProperty("result", out var resultElem))
                    {
                        tcs.SetResult(resultElem);
                    }
                    else if (root.TryGetProperty("error", out var errElem))
                    {
                        string errMsg = errElem.GetProperty("message").GetString() ?? "Unknown error";
                        tcs.SetException(new Exception($"MCP error: {errMsg}"));
                    }
                }
            }
        }

        private async Task HandleServerRequestAsync(string idRaw, string method, JsonElement paramElem)
        {
            _logger?.LogDebug("Client received request '{0}' from server", method);
            if (method == "sampling/createMessage")
            {
                if (OnSamplingRequest == null)
                {
                    // Client isn't prepared to handle sampling -> return error
                    await SendErrorAsync(idRaw, -32601, "Sampling not supported by client");
                }
                else
                {
                    // Parse incoming sampling request
                    var messages = new List<ChatMessage>();
                    string? systemPrompt = null;
                    if (paramElem.TryGetProperty("messages", out var msgsElem) && msgsElem.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var msgElem in msgsElem.EnumerateArray())
                        {
                            var msg = JsonSerializer.Deserialize<ChatMessage>(msgElem.GetRawText());
                            if (msg != null) messages.Add(msg);
                        }
                    }
                    if (paramElem.TryGetProperty("systemPrompt", out var sysElem))
                    {
                        systemPrompt = sysElem.GetString();
                    }
                    ChatMessage reply;
                    try
                    {
                        // Invoke host's handler to get completion
                        reply = await OnSamplingRequest(messages, systemPrompt);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Exception in OnSamplingRequest handler");
                        await SendErrorAsync(idRaw, -32603, "Sampling handler exception", ex.Message);
                        return;
                    }
                    await SendResponseAsync(idRaw, new { message = reply });
                }
            }
            else
            {
                await SendErrorAsync(idRaw, -32601, "Method not found");
            }
        }

        private Task HandleServerNotificationAsync(string method, JsonElement paramElem)
        {
            _logger?.LogDebug("Client received notification '{0}'", method);
            if (method == "resources/updated")
            {
                string? uri = paramElem.TryGetProperty("uri", out var uriProp) ? uriProp.GetString() : null;
                _logger?.LogInformation("Resource updated notification: {0}", uri);
                // (Application could hook here to react to changes)
            }
            else if (method == "progress")
            {
                if (paramElem.TryGetProperty("current", out var cur) && paramElem.TryGetProperty("total", out var tot))
                {
                    _logger?.LogInformation("Progress update: {0}/{1}", cur.GetInt32(), tot.GetInt32());
                }
            }
            // Handle other notifications like roots/updated if needed
            return Task.CompletedTask;
        }

        // Manage pending client requests
        private int _nextRequestId = 1;
        private readonly Dictionary<string, TaskCompletionSource<JsonElement>> _pendingRequests = new Dictionary<string, TaskCompletionSource<JsonElement>>();

        private async Task<JsonElement> SendRequestAsync(string method, object? paramObj)
        {
            if (_transport == null) throw new InvalidOperationException("Client not connected");
            string id = _nextRequestId++.ToString();
            var req = new { jsonrpc = "2.0", id = id, method = method, @params = paramObj };
            string json = JsonSerializer.Serialize(req);
            var tcs = new TaskCompletionSource<JsonElement>();
            _pendingRequests[id] = tcs;
            await _transport.SendAsync(json);
            return await tcs.Task;
        }
        private async Task SendNotificationAsync(string method, object? paramObj)
        {
            if (_transport == null) throw new InvalidOperationException("Client not connected");
            var note = new { jsonrpc = "2.0", method = method, @params = paramObj };
            string json = JsonSerializer.Serialize(note);
            await _transport.SendAsync(json);
        }
        private async Task SendResponseAsync(string idRaw, object resultObj)
        {
            if (_transport == null) return;
            JsonElement idElem = JsonDocument.Parse(idRaw).RootElement;
            var response = new { jsonrpc = "2.0", id = idElem, result = resultObj };
            string json = JsonSerializer.Serialize(response);
            await _transport.SendAsync(json);
        }
        private async Task SendErrorAsync(string idRaw, int code, string message, string? data = null)
        {
            if (_transport == null) return;
            JsonElement idElem = JsonDocument.Parse(idRaw).RootElement;
            var errorObj = new { code = code, message = message, data = data };
            var response = new { jsonrpc = "2.0", id = idElem, error = errorObj };
            string json = JsonSerializer.Serialize(response);
            await _transport.SendAsync(json);
        }

        // Public client-side API:
        public async Task<List<ResourceInfo>> ListResourcesAsync()
        {
            JsonElement result = await SendRequestAsync("resources/list", new { });
            var resourcesElem = result.GetProperty("resources");
            return JsonSerializer.Deserialize<List<ResourceInfo>>(resourcesElem.GetRawText()) ?? new List<ResourceInfo>();
        }
        public async Task<(byte[]? data, string? mimeType)> ReadResourceAsync(string uri)
        {
            JsonElement result = await SendRequestAsync("resources/read", new { uri = uri });
            var contents = result.GetProperty("contents");
            byte[]? data = null;
            string? mime = null;
            // Take first content item (if any)
            foreach (var item in contents.EnumerateArray())
            {
                if (item.TryGetProperty("text", out var textElem))
                {
                    string text = textElem.GetString() ?? "";
                    data = Encoding.UTF8.GetBytes(text);
                    mime = item.TryGetProperty("mimeType", out var mElem) ? mElem.GetString() : "text/plain";
                }
                if (item.TryGetProperty("data", out var dataElem))
                {
                    data = dataElem.GetBytesFromBase64();
                    mime = item.TryGetProperty("mimeType", out var mElem) ? mElem.GetString() : "application/octet-stream";
                }
                break;
            }
            return (data, mime);
        }
        public async Task SubscribeResourceAsync(string uri)
        {
            await SendRequestAsync("resources/subscribe", new { uri = uri });
        }
        public async Task UnsubscribeResourceAsync(string uri)
        {
            await SendRequestAsync("resources/unsubscribe", new { uri = uri });
        }
        public async Task<List<ToolInfo>> ListToolsAsync()
        {
            JsonElement result = await SendRequestAsync("tools/list", new { });
            var toolsElem = result.GetProperty("tools");
            return JsonSerializer.Deserialize<List<ToolInfo>>(toolsElem.GetRawText()) ?? new List<ToolInfo>();
        }
        public async Task<ContentItem[]> CallToolAsync(string toolName, Dictionary<string, object> args)
        {
            // Serialize args to JsonElement
            string argsJson = JsonSerializer.Serialize(args);
            JsonElement argsElement = JsonDocument.Parse(argsJson).RootElement;
            JsonElement result = await SendRequestAsync("tools/call", new { tool = toolName, args = argsElement });
            var contentElem = result.GetProperty("content");
            return JsonSerializer.Deserialize<ContentItem[]>(contentElem.GetRawText()) ?? Array.Empty<ContentItem>();
        }
        public async Task<List<PromptInfo>> ListPromptsAsync()
        {
            JsonElement result = await SendRequestAsync("prompts/list", new { });
            var promptsElem = result.GetProperty("prompts");
            return JsonSerializer.Deserialize<List<PromptInfo>>(promptsElem.GetRawText()) ?? new List<PromptInfo>();
        }
        public async Task<List<ChatMessage>> GetPromptAsync(string promptName, Dictionary<string, object>? args = null)
        {
            JsonElement argsElem;
            if (args != null)
            {
                string argsJson = JsonSerializer.Serialize(args);
                argsElem = JsonDocument.Parse(argsJson).RootElement;
            }
            else
            {
                argsElem = JsonDocument.Parse("{}").RootElement;
            }
            JsonElement result = await SendRequestAsync("prompts/get", new { prompt = promptName, args = argsElem });
            var messagesElem = result.GetProperty("messages");
            return JsonSerializer.Deserialize<List<ChatMessage>>(messagesElem.GetRawText()) ?? new List<ChatMessage>();
        }
    }
}