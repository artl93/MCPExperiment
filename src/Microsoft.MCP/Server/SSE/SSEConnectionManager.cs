using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Microsoft.MCP.Server.SSE
{
    /// <summary>
    /// Manages SSE connections to clients.
    /// </summary>
    public class SSEConnectionManager : IDisposable
    {
        private readonly ConcurrentDictionary<string, SSEConnection> _connections = new();
        private readonly ILogger<SSEConnectionManager> _logger;
        private readonly Timer _keepAliveTimer;
        private bool _disposed;
        
        /// <summary>
        /// Gets the current number of active connections.
        /// </summary>
        public int ConnectionCount => _connections.Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="SSEConnectionManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public SSEConnectionManager(ILogger<SSEConnectionManager> logger)
        {
            _logger = logger;
            
            // Set up a timer to send keep-alive messages every 30 seconds
            _keepAliveTimer = new Timer(SendKeepAlive, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// Adds a connection to be managed.
        /// </summary>
        /// <param name="connection">The connection to add.</param>
        public void AddConnection(SSEConnection connection)
        {
            if (_disposed) return;

            _connections[connection.ConnectionId] = connection;
            _logger.LogDebug("Added SSE connection {ConnectionId}, total connections: {ConnectionCount}", 
                connection.ConnectionId, _connections.Count);
        }

        /// <summary>
        /// Removes a connection from management.
        /// </summary>
        /// <param name="connectionId">The ID of the connection to remove.</param>
        public void RemoveConnection(string connectionId)
        {
            if (_disposed) return;

            if (_connections.TryRemove(connectionId, out var connection))
            {
                connection.Dispose();
                _logger.LogDebug("Removed SSE connection {ConnectionId}, remaining connections: {ConnectionCount}", 
                    connectionId, _connections.Count);
            }
        }

        /// <summary>
        /// Sends an event to all connected clients.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        /// <param name="data">The event data.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SendEventToAllAsync(string eventName, string data)
        {
            if (_disposed) return;

            var disconnectedConnections = new List<string>();
            int successCount = 0;

            // Debug log at the start
            _logger.LogInformation("Sending '{EventName}' event to {ConnectionCount} SSE clients", 
                eventName, _connections.Count);
            
            if (_connections.Count == 0)
            {
                _logger.LogWarning("No active SSE connections to send events to");
                return;
            }

            foreach (var connection in _connections.Values)
            {
                try
                {
                    if (connection.CancellationToken.IsCancellationRequested)
                    {
                        _logger.LogDebug("Connection {ConnectionId} has cancellation requested, skipping", connection.ConnectionId);
                        disconnectedConnections.Add(connection.ConnectionId);
                        continue;
                    }

                    _logger.LogDebug("Sending event '{EventName}' to connection {ConnectionId}", eventName, connection.ConnectionId);
                    await connection.SendEventAsync(eventName, data);
                    successCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending event to connection {ConnectionId}", connection.ConnectionId);
                    disconnectedConnections.Add(connection.ConnectionId);
                }
            }

            // Clean up disconnected connections
            foreach (var connectionId in disconnectedConnections)
            {
                RemoveConnection(connectionId);
            }
            
            _logger.LogInformation("Successfully sent '{EventName}' event to {SuccessCount} connections", 
                eventName, successCount);
        }

        private void SendKeepAlive(object? state)
        {
            if (_disposed) return;

            // Fire and forget
            Task.Run(async () =>
            {
                var disconnectedConnections = new List<string>();

                foreach (var connection in _connections.Values)
                {
                    try
                    {
                        if (connection.CancellationToken.IsCancellationRequested)
                        {
                            disconnectedConnections.Add(connection.ConnectionId);
                            continue;
                        }

                        await connection.SendKeepAliveAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error sending keep-alive to connection {ConnectionId}", connection.ConnectionId);
                        disconnectedConnections.Add(connection.ConnectionId);
                    }
                }

                // Clean up disconnected connections
                foreach (var connectionId in disconnectedConnections)
                {
                    RemoveConnection(connectionId);
                }
            });
        }

        /// <summary>
        /// Disposes resources used by the connection manager.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _keepAliveTimer.Dispose();
                
                foreach (var connection in _connections.Values)
                {
                    connection.Dispose();
                }
                
                _connections.Clear();
                _disposed = true;
            }
        }
    }
}