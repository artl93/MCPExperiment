using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Extensions.ModelContextProtocol
{
    public class ModelContextProtocolClient
    {
        private readonly HttpClient _httpClient;
        public string ServerUrl { get; }

        public ModelContextProtocolClient(string serverUrl)
        {
            if (string.IsNullOrWhiteSpace(serverUrl))
                throw new ArgumentNullException(nameof(serverUrl));
            ServerUrl = serverUrl;
            // Initialize HttpClient with the provided server URL.
            _httpClient = new HttpClient { BaseAddress = new Uri(serverUrl) };
        }

        // Checks connectivity to the MCP server, for example via a health-check endpoint.
        public async Task<bool> ConnectAsync()
        {
            try
            {
                // Assuming the MCP server provides a /health endpoint.
                var response = await _httpClient.GetAsync("/health");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // Sends a message to the MCP server and returns the server's response.
        public async Task<string> SendMessageAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));

            // In a real implementation, the message might be wrapped in a protocol-specific structure.
            var content = new StringContent(message);
            var response = await _httpClient.PostAsync("/mcp", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
