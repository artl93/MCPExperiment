using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.MCP.Annotations;
using Microsoft.Extensions.MCP.Protocol;

namespace Microsoft.Extensions.MCP.Protocol.JsonRpc.Serialization
{
    internal class SamplingMessageConverter : JsonConverter<SamplingMessage>
    {
        public override SamplingMessage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var jsonDoc = JsonDocument.ParseValue(ref reader);
            var root = jsonDoc.RootElement;

            // Based on schema.ts.txt: "role" is required
            if (!root.TryGetProperty("role", out var roleProperty))
                throw new JsonException("Missing 'role' property in SamplingMessage");
            string role = roleProperty.GetString()!;

            // Based on schema.ts.txt: "content" may now include audio content.
            if (!root.TryGetProperty("content", out var contentProperty))
                throw new JsonException("Missing 'content' property in SamplingMessage");

            object content = null!;
            if (contentProperty.ValueKind == JsonValueKind.Object)
            {
                if (!contentProperty.TryGetProperty("type", out var typeProp))
                    throw new JsonException("Missing 'type' in content");

                string contentType = typeProp.GetString()!;
                content = contentType switch
                {
                    "text" => JsonSerializer.Deserialize<TextContent>(contentProperty.GetRawText(), options)!,
                    "image" => JsonSerializer.Deserialize<ImageContent>(contentProperty.GetRawText(), options)!,
                    "audio" => JsonSerializer.Deserialize<AudioContent>(contentProperty.GetRawText(), options)!,
                    _ => throw new JsonException($"Unknown content type: {contentType}")
                };
            }

            return new CreateMessageResult { Role = role, Content = content };
        }

        public override void Write(Utf8JsonWriter writer, SamplingMessage value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("role", value.Role);
            writer.WritePropertyName("content");
            JsonSerializer.Serialize(writer, value.Content, value.Content.GetType(), options);
            writer.WriteEndObject();
        }
    }
}
