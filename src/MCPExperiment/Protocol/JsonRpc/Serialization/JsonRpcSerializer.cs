using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCPExperiment.Protocol.JsonRpc.Serialization
{
    public static class JsonRpcSerializer
    {
        public static JsonSerializerOptions CreateOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            };

            // Based on schema.ts.txt: Register converters for JSON-RPC types.
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            // options.Converters.Add(new ContentConverter()); // Removed if not needed per revised schema.
            options.Converters.Add(new SamplingMessageConverter());
            // ...existing converters...

            // >>> REMOVED: Registration for ListResourceTemplatesRequest and ListResourceTemplatesResult converters per draft schema.ts.txt

            return options;
        }
    }
}
