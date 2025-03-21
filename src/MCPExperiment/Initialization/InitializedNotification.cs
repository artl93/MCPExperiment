using MCPExperiment.Messages;

namespace MCPExperiment.Initialization
{
    /// <summary>
    /// This notification is sent from the client to the server after initialization has finished.
    /// </summary>
    public class InitializedNotification : JsonRpcNotification<object>, IClientNotification
    {
        /// <summary>
        /// Creates a new instance of the InitializedNotification class.
        /// </summary>
        public InitializedNotification() => Method = "notifications/initialized";
    }
}
