using Microsoft.Extensions.AI.MCP.Capabilities;
using Microsoft.Extensions.AI.MCP.Messages;
using Microsoft.Extensions.AI.MCP.Server.Models;
using System.Collections.Generic;

namespace Microsoft.Extensions.AI.MCP.Resources
{
    /// <summary>
    /// Notification sent when the list of available tools has changed.
    /// Clients can use this to update their cached list of tools without requiring re-initialization.
    /// </summary>
    public class ToolListChangedNotification : IServerNotification
    {
        /// <summary>
        /// Gets the method name for this notification.
        /// </summary>
        public string Method => "notifications/tools/listChanged";

        /// <summary>
        /// Gets or sets the parameters for this notification.
        /// </summary>
        public ToolListChangedParams Params { get; set; } = new ToolListChangedParams();
    }

    /// <summary>
    /// Parameters for the tool list changed notification.
    /// </summary>
    public class ToolListChangedParams
    {
        /// <summary>
        /// Gets or sets the list of available tools.
        /// </summary>
        public List<ToolDefinition>? AvailableTools { get; set; }
    }
}