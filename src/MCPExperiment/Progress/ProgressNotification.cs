using MCPExperiment.Messages;

namespace MCPExperiment.Progress
{
    /// <summary>
    /// An out-of-band notification used to inform the receiver of a progress update for a long-running request.
    /// </summary>
    public class ProgressNotification : JsonRpcNotification<ProgressNotificationParams>, Messages.IClientNotification, Messages.IServerNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressNotification"/> class.
        /// </summary>
        public ProgressNotification() => Method = "notifications/progress";
    }
}
