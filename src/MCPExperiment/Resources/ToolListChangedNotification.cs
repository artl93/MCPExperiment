using MCPExperiment.Capabilities;
using MCPExperiment.Messages;
using MCPExperiment.Server.Models;
using System.Collections.Generic;

namespace MCPExperiment.Resources
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