using System;
using System.Threading.Tasks;

namespace MCP
{
    /// <summary>Transport interface for MCP communication.</summary>
    public interface IMcpTransport
    {
        Task StartAsync(Func<string, Task> onMessageReceived);
        Task SendAsync(string message);
        Task StopAsync();
    }
}