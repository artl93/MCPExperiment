using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.MCP.Initialization;
using MCPCaps = Microsoft.MCP.Capabilities;

namespace MCP
{
    /// <summary>Represents an MCP server (to expose resources, tools, prompts).</summary>
    public class McpServer
    {
        private readonly MCPCaps.Implementation _serverInfo;
        private readonly MCPCaps.ServerCapabilities _capabilities = new();
        private IMcpTransport? _transport;
        private readonly ILogger? _logger;
        private bool _initialized = false;
        private MCPCaps.Implementation? _clientInfo;
        private MCPCaps.ClientCapabilities? _clientCaps;

        // Delegate definitions for handlers:
        private class ResourceHandler
        {
            public string UriPattern = "";  // e.g. exact URI or pattern with {param}
            public required Func<Uri, Dictionary<string, string>, McpContext, Task<object?>> Invoke;
            public string? Name;
            public string? Description;
            public string? MimeType;
            public bool IsDynamic => UriPattern.Contains("{");
        }
        private class ToolHandler
        {
            public string Name = "";
            public string? Description;
            public required Func<Dictionary<string, JsonElement>, McpContext, Task<object?>> Invoke;
        }
        private class PromptHandler
        {
            public string Name = "";
            public string? Description;
            public required Func<Dictionary<string, JsonElement>, McpContext, Task<object?>> Invoke;
        }

        private readonly List<ResourceHandler> _resources = new List<ResourceHandler>();
        private readonly List<ToolHandler> _tools = new List<ToolHandler>();
        private readonly List<PromptHandler> _prompts = new List<PromptHandler>();

        public McpServer(string name, string version, ILogger? logger = null)
        {
            _serverInfo = new () { Name = name, Version = version };
            _logger = logger;
        }

        /// <summary>Add a static resource with a fixed URI.</summary>
        public void AddResource(string uri, string name, string description, Func<McpContext, Task<object?>> handler)
        {
            _resources.Add(new ResourceHandler
            {
                UriPattern = uri,
                Name = name,
                Description = description,
                Invoke = (u, pars, ctx) => handler(ctx),
                MimeType = null
            });
        }
        /// <summary>Add a dynamic resource with a URI pattern containing parameters in braces (e.g. "file://{path}").</summary>
        public void AddResource(string uriTemplate, string name, string description, Func<Dictionary<string, string>, McpContext, Task<object?>> handler)
        {
            _resources.Add(new ResourceHandler
            {
                UriPattern = uriTemplate,
                Name = name,
                Description = description,
                Invoke = (u, pars, ctx) => handler(pars, ctx),
                MimeType = null
            });
        }
        // Overloads to allow synchronous lambdas (automatically wrapped to Task):
        public void AddResource(string uri, string name, string description, Func<McpContext, object?> handler)
            => AddResource(uri, name, description, ctx => Task.FromResult(handler(ctx)));
        public void AddResource(string uriTemplate, string name, string description, Func<Dictionary<string, string>, McpContext, object?> handler)
            => AddResource(uriTemplate, name, description, (pars, ctx) => Task.FromResult(handler(pars, ctx)));

        /// <summary>Add a tool (function) that can be called by name with JSON arguments.</summary>
        public void AddTool(string name, string description, Func<Dictionary<string, JsonElement>, McpContext, Task<object?>> handler)
        {
            _tools.Add(new ToolHandler { Name = name, Description = description, Invoke = handler });
        }
        public void AddTool(string name, string description, Func<Dictionary<string, JsonElement>, McpContext, object?> handler)
            => AddTool(name, description, (args, ctx) => Task.FromResult(handler(args, ctx)));

        /// <summary>Add a prompt template that can be retrieved by name.</summary>
        public void AddPrompt(string name, string description, Func<Dictionary<string, JsonElement>, McpContext, Task<object?>> handler)
        {
            _prompts.Add(new PromptHandler { Name = name, Description = description, Invoke = handler });
        }
        public void AddPrompt(string name, string description, Func<Dictionary<string, JsonElement>, McpContext, object?> handler)
            => AddPrompt(name, description, (args, ctx) => Task.FromResult(handler(args, ctx)));

        /// <summary>Start the server by attaching it to a transport (e.g. StdioTransport).</summary>
        public async Task ConnectAsync(IMcpTransport transport)
        {
            _transport = transport;
            // Set capability flags based on what's added
            if (_resources.Count > 0)
                _capabilities.Resources = new () { Subscribe = true, ListChanged = true };
            if (_tools.Count > 0){ _capabilities.Tools = new ();
                // add the tools list 
                foreach (var tool in _tools)
                {
                    _capabilities.Tools.AvailableTools.Add(new () { Name = tool.Name, Description = tool.Description });
                }
            }
            if (_prompts.Count > 0)
            {
                 _capabilities.Prompts = new ();
                // add the prompts list
                foreach (var prompt in _prompts)
                {
                    _capabilities.Prompts.AvailablePrompts.Add(new () { Name = prompt.Name, Description = prompt.Description });
                }
            }
            // (Sampling capability could be set if server intends to use it - here we assume server can request if client supports.)
            _logger?.LogDebug("MCP server starting (name={0}, version={1})", _serverInfo.Name, _serverInfo.Version);



            // Start listening for messages
            await _transport.StartAsync(async msg =>
            {
                try
                {
                    await ProcessMessageAsync(msg);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error processing MCP server message");
                }
            });
        }

        private async Task ProcessMessageAsync(string jsonMessage)
        {
            using JsonDocument doc = JsonDocument.Parse(jsonMessage);
            JsonElement root = doc.RootElement;
            if (root.TryGetProperty("method", out var methodElem))
            {
                // It's a request or notification from client
                string method = methodElem.GetString()!;
                if (root.TryGetProperty("id", out var idElem))
                {
                    // request (with ID)
                    string idRaw = idElem.GetRawText(); // raw to preserve number or string type
                    JsonElement paramsElem = root.GetProperty("params");
                    await HandleRequestAsync(idRaw, method, paramsElem);
                }
                else
                {
                    // notification (no ID)
                    JsonElement paramsElem = root.TryGetProperty("params", out var pElem) ? pElem : default;
                    await HandleNotificationAsync(method, paramsElem);
                }
            }
            else if (root.TryGetProperty("id", out var respIdElem))
            {
                // It's a response to a request this server sent (e.g. sampling)
                string respIdRaw = respIdElem.GetRawText();
                if (_pendingRequests.TryGetValue(respIdRaw, out var pending))
                {
                    _pendingRequests.Remove(respIdRaw);
                    if (root.TryGetProperty("result", out var resultElem))
                    {
                        pending.SetResult(resultElem);
                    }
                    else if (root.TryGetProperty("error", out var errorElem))
                    {
                        string errMsg = errorElem.GetProperty("message").GetString() ?? "Unknown error";
                        pending.SetException(new Exception($"MCP error: {errMsg}"));
                    }
                }
            }
        }

        private async Task HandleRequestAsync(string idRaw, string method, JsonElement paramElem)
        {
            _logger?.LogDebug("Server received request '{0}' (ID {1})", method, idRaw);
            if (method == "initialize")
            {
                // Host is initiating handshake
                var initParams = JsonSerializer.Deserialize<InitializeRequestParams>(paramElem.GetRawText()) ?? new InitializeRequestParams();
                _clientInfo = new ()
                {
                    Name = initParams.ClientInfo.Name,
                    Version = initParams.ClientInfo.Version
                };
                _clientCaps = initParams.Capabilities;
                // Respond with server info and capabilities
                var initResult = new Microsoft.MCP.Initialization.InitializeResult ()
                { 
                    ServerInfo  = _serverInfo, 
                    Capabilities = _capabilities, 
                    ProtocolVersion = "1.0"
                };
                await SendResponseAsync(idRaw, initResult);
                return;
            }
            if (!_initialized && method == "initialized")
            {
                // Final step of handshake (client acknowledgment)
                _initialized = true;
                _logger?.LogInformation("MCP server handshake complete (client: {0} v{1})", _clientInfo?.Name, _clientInfo?.Version);
                return;
            }
            if (!_initialized)
            {
                // If we get other requests before initialization is done, respond with error.
                await SendErrorAsync(idRaw, -32002, "Server not initialized");
                return;
            }

            if (method == "resources/list")
            {
                var resList = new List<ResourceInfo>();
                foreach (var res in _resources)
                {
                    if (!res.IsDynamic)
                    {
                        resList.Add(new ResourceInfo { Uri = res.UriPattern, Name = res.Name, Description = res.Description, MimeType = res.MimeType });
                    }
                    // (Dynamic resources could provide a listing if supported - not implemented here.)
                }
                await SendResponseAsync(idRaw, new { resources = resList });
            }
            else if (method == "resources/read")
            {
                string? uri = paramElem.TryGetProperty("uri", out var uriProp) ? uriProp.GetString() : null;
                if (uri == null)
                {
                    await SendErrorAsync(idRaw, -32602, "Invalid params for resources/read");
                    return;
                }
                Uri uriObj;
                try { uriObj = new Uri(uri); }
                catch {
                    await SendErrorAsync(idRaw, -32600, "Malformed URI");
                    return;
                }
                ResourceHandler? handler = null;
                var pathParams = new Dictionary<string, string>();
                foreach (var res in _resources)
                {
                    if (res.IsDynamic)
                    {
                        if (TryMatchUriPattern(res.UriPattern, uriObj, out var extracted))
                        {
                            handler = res;
                            pathParams = extracted;
                            break;
                        }
                    }
                    else if (uri.Equals(res.UriPattern, StringComparison.OrdinalIgnoreCase))
                    {
                        handler = res;
                        break;
                    }
                }
                if (handler == null)
                {
                    await SendErrorAsync(idRaw, -32001, "Resource not found");
                    return;
                }
                var ctx = new McpContext(server: this, client: null, logger: _logger, requestId: idRaw);
                object? handlerResult;
                try
                {
                    handlerResult = await handler.Invoke(uriObj, pathParams, ctx);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Exception in resource handler for {0}", uri);
                    await SendErrorAsync(idRaw, -32603, "Resource handler exception", ex.Message);
                    return;
                }
                // Convert handler result to standard resource read result
                ContentItem contentItem;
                if (handlerResult is ContentItem ci) contentItem = ci;
                else if (handlerResult is string s) contentItem = new ContentItem { Uri = uri, Text = s };
                else if (handlerResult is byte[] data) contentItem = new ContentItem { Uri = uri, Data = data, MimeType = handler.MimeType ?? "application/octet-stream" };
                else if (handlerResult is ChatMessage chatMsg) contentItem = chatMsg.Content;
                else contentItem = new ContentItem { Uri = uri, Text = handlerResult != null ? JsonSerializer.Serialize(handlerResult) : "" };

                await SendResponseAsync(idRaw, new { contents = new[] { contentItem } });
            }
            else if (method == "resources/subscribe")
            {
                // Subscribe to resource changes. We acknowledge but actual tracking not implemented here.
                await SendResponseAsync(idRaw, new object());
            }
            else if (method == "resources/unsubscribe")
            {
                await SendResponseAsync(idRaw, new object());
            }
            else if (method == "tools/list")
            {
                var toolList = new List<ToolInfo>();
                foreach (var t in _tools)
                {
                    toolList.Add(new ToolInfo { Name = t.Name, Description = t.Description });
                }
                await SendResponseAsync(idRaw, new { tools = toolList });
            }
            else if (method == "tools/call")
            {
                string? toolName = paramElem.TryGetProperty("tool", out var toolProp) ? toolProp.GetString() : null;
                JsonElement argsElem = paramElem.TryGetProperty("args", out var argsProp) ? argsProp : default;
                if (toolName == null || argsElem.ValueKind != JsonValueKind.Object)
                {
                    await SendErrorAsync(idRaw, -32602, "Invalid params for tools/call");
                    return;
                }
                var tool = _tools.Find(t => t.Name == toolName);
                if (tool == null)
                {
                    await SendErrorAsync(idRaw, -32001, "Tool not found");
                    return;
                }
                // Convert JsonElement arguments to dictionary
                var argsDict = new Dictionary<string, JsonElement>();
                foreach (var prop in argsElem.EnumerateObject())
                {
                    argsDict[prop.Name] = prop.Value;
                }
                var ctx = new McpContext(server: this, client: null, logger: _logger, requestId: idRaw);
                object? result;
                try
                {
                    result = await tool.Invoke(argsDict, ctx);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Exception in tool handler '{0}'", toolName);
                    await SendErrorAsync(idRaw, -32603, "Tool handler exception", ex.Message);
                    return;
                }
                ContentItem contentItem;
                if (result is ContentItem ci) contentItem = ci;
                else if (result is string str) contentItem = new ContentItem { Type = "text", Text = str };
                else if (result is byte[] bin) contentItem = new ContentItem { Type = "binary", Data = bin, MimeType = "application/octet-stream" };
                else if (result is ChatMessage cm) contentItem = cm.Content;
                else contentItem = new ContentItem { Type = "text", Text = result != null ? JsonSerializer.Serialize(result) : "" };

                await SendResponseAsync(idRaw, new { content = new[] { contentItem } });
            }
            else if (method == "prompts/list")
            {
                var promptList = new List<PromptInfo>();
                foreach (var p in _prompts)
                {
                    promptList.Add(new PromptInfo { Name = p.Name, Description = p.Description });
                }
                await SendResponseAsync(idRaw, new { prompts = promptList });
            }
            else if (method == "prompts/get")
            {
                string? promptName = paramElem.TryGetProperty("prompt", out var prProp) ? prProp.GetString() : null;
                JsonElement argsElem = paramElem.TryGetProperty("args", out var prArgsProp) ? prArgsProp : default;
                if (promptName == null)
                {
                    await SendErrorAsync(idRaw, -32602, "Invalid params for prompts/get");
                    return;
                }
                var prompt = _prompts.Find(p => p.Name == promptName);
                if (prompt == null)
                {
                    await SendErrorAsync(idRaw, -32001, "Prompt not found");
                    return;
                }
                var argsDict = new Dictionary<string, JsonElement>();
                if (argsElem.ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in argsElem.EnumerateObject())
                        argsDict[prop.Name] = prop.Value;
                }
                var ctx = new McpContext(server: this, client: null, logger: _logger, requestId: idRaw);
                object? result;
                try
                {
                    result = await prompt.Invoke(argsDict, ctx);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Exception in prompt handler '{0}'", promptName);
                    await SendErrorAsync(idRaw, -32603, "Prompt handler exception", ex.Message);
                    return;
                }
                List<ChatMessage> messages;
                if (result is ChatMessage singleMsg) messages = new List<ChatMessage> { singleMsg };
                else if (result is IEnumerable<ChatMessage> msgList) messages = new List<ChatMessage>(msgList);
                else if (result is string s) 
                    messages = new List<ChatMessage> { new ChatMessage { Role = "user", Content = new ContentItem { Text = s, Type = "text" } } };
                else 
                    messages = new List<ChatMessage>(); // default to empty if unsupported type
                await SendResponseAsync(idRaw, new { messages = messages });
            }
            else if (method.StartsWith("sampling/"))
            {
                // The only expected server->client sampling request is "sampling/createMessage", not something client should send to server.
                await SendErrorAsync(idRaw, -32601, "Method not found");
            }
            else
            {
                await SendErrorAsync(idRaw, -32601, "Method not found");
            }
        }

        private async Task HandleNotificationAsync(string method, JsonElement paramElem)
        {
            _logger?.LogDebug("Server received notification '{0}'", method);
            if (method == "initialized")
            {
                _initialized = true;
                _logger?.LogInformation("Server initialized by client");
            }
            else if (method == "cancel")
            {
                // Client can signal cancellation of a request (not fully implemented in this sample).
                _logger?.LogInformation("Server received cancellation request");
            }
            else if (method == "roots/updated")
            {
                // Client updated roots scope (if supported)
                _logger?.LogInformation("Server notified of roots update");
            }
            // (Handle other notifications like progress if needed.)
            
            // Add a simple await to satisfy the async method requirement
            await Task.CompletedTask;
        }

        // Utility: match a URI pattern with {placeholders} against an actual URI, extracting parameters.
        private bool TryMatchUriPattern(string pattern, Uri uri, out Dictionary<string, string> parameters)
        {
            parameters = new Dictionary<string, string>();
            string uriStr = uri.ToString();
            int iPattern = 0, iUri = 0;
            while (iPattern < pattern.Length && iUri < uriStr.Length)
            {
                if (pattern[iPattern] == '{')
                {
                    int endBrace = pattern.IndexOf('}', iPattern);
                    if (endBrace < 0) break;
                    string paramName = pattern.Substring(iPattern + 1, endBrace - iPattern - 1);
                    // Determine next literal part in pattern after this placeholder
                    iPattern = endBrace + 1;
                    string nextLiteral = "";
                    if (iPattern < pattern.Length)
                    {
                        // Next literal part is up to next '{' or end of pattern
                        int nextBrace = pattern.IndexOf('{', iPattern);
                        nextLiteral = nextBrace >= 0 ? pattern.Substring(iPattern, nextBrace - iPattern) : pattern[iPattern..];
                    }
                    else
                    {
                        nextLiteral = "";
                    }
                    int nextIdx = nextLiteral == "" ? uriStr.Length : uriStr.IndexOf(nextLiteral, iUri, StringComparison.Ordinal);
                    if (nextIdx < 0) return false;
                    string value = uriStr.Substring(iUri, nextIdx - iUri);
                    parameters[paramName] = Uri.UnescapeDataString(value);
                    iUri = nextIdx + nextLiteral.Length;
                }
                else
                {
                    if (uriStr[iUri] != pattern[iPattern]) return false;
                    iUri++; 
                    iPattern++;
                }
            }
            return iPattern == pattern.Length && iUri == uriStr.Length;
        }

        // Methods to send responses or requests from server:
        private async Task SendResponseAsync(string idRaw, object resultObj)
        {
            if (_transport == null) return;
            // Preserve the ID type (number or string) by parsing raw JSON text
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

        // For server-initiated requests (like sampling) we manage a dictionary of pending requests:
        private int _nextRequestId = 1;
        private readonly Dictionary<string, TaskCompletionSource<JsonElement>> _pendingRequests = new Dictionary<string, TaskCompletionSource<JsonElement>>();

        internal async Task<JsonElement> SendRequestAsync(string method, object? paramObj)
        {
            if (_transport == null) throw new InvalidOperationException("Server transport not connected");
            string id = _nextRequestId++.ToString();
            var request = new { jsonrpc = "2.0", id = id, method = method, @params = paramObj };
            string json = JsonSerializer.Serialize(request);
            var tcs = new TaskCompletionSource<JsonElement>();
            _pendingRequests[id] = tcs;
            await _transport.SendAsync(json);
            return await tcs.Task; // await response
        }
        internal async Task SendNotificationAsync(string method, object? paramObj)
        {
            if (_transport == null) throw new InvalidOperationException("Server transport not connected");
            var notification = new { jsonrpc = "2.0", method = method, @params = paramObj };
            string json = JsonSerializer.Serialize(notification);
            await _transport.SendAsync(json);
        }
    }
}