using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.AI.MCP.Union
{
    /// <summary>
    /// JSON converter for <see cref="UnionValue"/> that handles serialization and deserialization 
    /// of values that can be either string or number types.
    /// </summary>
    /// <remarks>
    /// This converter enables Model Context Protocol to handle fields like RequestId which can
    /// be either a string or a number according to the JSON-RPC specification.
    /// </remarks>
    public class UnionJsonConverter : JsonConverter<UnionValue>
    {
        /// <summary>
        /// Reads and converts the JSON to a <see cref="UnionValue"/>.
        /// </summary>
        /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
        /// <param name="typeToConvert">The <see cref="Type"/> being converted.</param>
        /// <param name="options">The <see cref="JsonSerializerOptions"/> being used.</param>
        /// <returns>A <see cref="UnionValue"/> representing either a string or number.</returns>
        /// <exception cref="JsonException">Thrown when the JSON is not a string or number.</exception>
        public override UnionValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
                return new UnionValue(reader.GetString() ?? string.Empty);
            else if (reader.TokenType == JsonTokenType.Number)
                return new UnionValue(reader.GetInt64());
            throw new JsonException("Expected string or number for union type.");
        }

        /// <summary>
        /// Writes a <see cref="UnionValue"/> as JSON.
        /// </summary>
        /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
        /// <param name="value">The <see cref="UnionValue"/> to convert.</param>
        /// <param name="options">The <see cref="JsonSerializerOptions"/> being used.</param>
        public override void Write(Utf8JsonWriter writer, UnionValue value, JsonSerializerOptions options)
        {
            if (value.Value is long l)
                writer.WriteNumberValue(l);
            else if (value.Value is string s)
                writer.WriteStringValue(s);
            else
                writer.WriteStringValue(value.ToString());
        }
    }
}
