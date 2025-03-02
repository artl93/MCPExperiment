using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Microsoft.Extensions.AI.MCP.Server.SSE
{
    /// <summary>
    /// Represents a Server-Sent Event (SSE) connection to a client.
    /// </summary>
    public class SSEConnection : IDisposable
    {
        private readonly HttpResponse _response;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private bool _disposed;

        /// <summary>
        /// Gets the unique identifier for this connection.
        /// </summary>
        public string ConnectionId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SSEConnection"/> class.
        /// </summary>
        /// <param name="response">The HTTP response object to write to.</param>
        public SSEConnection(HttpResponse response)
        {
            _response = response;
            ConnectionId = Guid.NewGuid().ToString();
            _cancellationTokenSource = new CancellationTokenSource();
            
            // Configure response for SSE
            _response.Headers["Content-Type"] = "text/event-stream";
            _response.Headers["Cache-Control"] = "no-cache";
            _response.Headers["Connection"] = "keep-alive";
        }

        /// <summary>
        /// Sends an event to the client.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        /// <param name="data">The event data.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SendEventAsync(string eventName, string data)
        {
            if (_disposed) return;

            try
            {
                await _response.WriteAsync($"event: {eventName}\n");
                
                // Format data with line breaks
                foreach (var line in data.Split('\n'))
                {
                    await _response.WriteAsync($"data: {line}\n");
                }
                
                await _response.WriteAsync("\n");
                await _response.Body.FlushAsync();
            }
            catch (Exception)
            {
                // Connection likely closed by client
                Cancel();
            }
        }

        /// <summary>
        /// Sends a comment to the client to keep the connection alive.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SendKeepAliveAsync()
        {
            if (_disposed) return;

            try
            {
                await _response.WriteAsync(": ping\n\n");
                await _response.Body.FlushAsync();
            }
            catch (Exception)
            {
                // Connection likely closed by client
                Cancel();
            }
        }

        /// <summary>
        /// Gets the cancellation token for this connection.
        /// </summary>
        public CancellationToken CancellationToken => _cancellationTokenSource.Token;

        /// <summary>
        /// Cancels this connection.
        /// </summary>
        public void Cancel()
        {
            if (!_disposed)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        /// <summary>
        /// Disposes resources used by this connection.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _cancellationTokenSource.Dispose();
                _disposed = true;
            }
        }
    }
}