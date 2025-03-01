using System.Text.Json.Serialization;
using Microsoft.Extensions.ModelContextProtocol.Messages;
using Microsoft.Extensions.ModelContextProtocol.Union;

namespace Microsoft.Extensions.ModelContextProtocol.Progress
{
    public class ProgressNotificationParams
    {
        [JsonPropertyName("progressToken")]
        public UnionValue ProgressToken { get; set; } = default!;

        [JsonPropertyName("progress")]
        public double Progress { get; set; }

        [JsonPropertyName("total")]
        public double? Total { get; set; }
    }

    public class ProgressNotification : JsonRpcNotification<ProgressNotificationParams>, Messages.IClientNotification, Messages.IServerNotification
    {
        public ProgressNotification() => Method = "notifications/progress";
    }
}
