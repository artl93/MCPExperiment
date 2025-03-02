using Microsoft.Extensions.MCP.Messages;

namespace Microsoft.Extensions.MCP.Resources
{
    /// <summary>
    /// An optional notification from the server to the client, informing it that the list of resources
    /// it can read from has changed. This may be issued by servers without any previous subscription
    /// from the client.
    /// </summary>
    public class ResourceListChangedNotification : JsonRpcNotification<object>, IServerNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceListChangedNotification"/> class.
        /// </summary>
        public ResourceListChangedNotification() => Method = "notifications/resources/list_changed";
    }
}
