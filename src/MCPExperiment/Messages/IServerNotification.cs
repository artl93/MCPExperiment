using System.Collections.Generic;
using System.Text.Json.Serialization;
using MCPExperiment;

namespace MCPExperiment.Messages
{
    /// <summary>
    /// Interface for all notification objects that can be sent from the server to the client.
    /// </summary>
    /// <remarks>
    /// Server notifications include CancelledNotification, ProgressNotification, LoggingMessageNotification,
    /// ResourceUpdatedNotification, ResourceListChangedNotification, ToolListChangedNotification,
    /// and PromptListChangedNotification.
    /// </remarks>
    public interface IServerNotification { }
}
