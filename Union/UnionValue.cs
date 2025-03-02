using System;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.AI.MCP.Union
{
    /// <summary>
    /// Represents a union type that can be either a string or a number.
    /// Used for fields like JSON-RPC RequestId which accepts multiple primitive types.
    /// </summary>
    [JsonConverter(typeof(UnionJsonConverter))]
    public readonly struct UnionValue
    {
        private readonly object _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnionValue"/> struct with a string value.
        /// </summary>
        /// <param name="value">The string value.</param>
        public UnionValue(string value) => _value = value;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnionValue"/> struct with a long value.
        /// </summary>
        /// <param name="value">The long value.</param>
        public UnionValue(long value) => _value = value;

        /// <summary>
        /// Implicitly converts a string to a <see cref="UnionValue"/>.
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        /// <returns>A new <see cref="UnionValue"/> containing the string.</returns>
        public static implicit operator UnionValue(string value) => new(value);

        /// <summary>
        /// Implicitly converts a long to a <see cref="UnionValue"/>.
        /// </summary>
        /// <param name="value">The long value to convert.</param>
        /// <returns>A new <see cref="UnionValue"/> containing the long.</returns>
        public static implicit operator UnionValue(long value) => new(value);

        /// <summary>
        /// Gets the underlying value.
        /// </summary>
        public object Value => _value;

        /// <summary>
        /// Returns a string representation of the value.
        /// </summary>
        /// <returns>A string representation of the contained value.</returns>
        public override string ToString() => _value?.ToString() ?? string.Empty;
    }
}
