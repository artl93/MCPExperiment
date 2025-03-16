using System;
using System.Threading.Tasks;

namespace Microsoft.MCP.Protocol.JsonRpc
{
    /// <summary>
    /// Transport interface for MCP communication.
    /// </summary>
    public interface IMCPTransport
    {
        /// <summary>
        /// Starts the transport with a handler for incoming messages.
        /// </summary>
        /// <param name="onMessageReceived">The handler for received messages.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task StartAsync(Func<string, Task> onMessageReceived);
        
        /// <summary>
        /// Sends a message through the transport.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendAsync(string message);
        
        /// <summary>
        /// Stops the transport.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task StopAsync();
    }
}