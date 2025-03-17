using System.Text.Json.Serialization;

namespace MCPExperiment.Annotations
{
    /// <summary>
    /// Represents the role of a participant in a conversation.
    /// </summary>
    /// <remarks>
    /// Used to distinguish between different participants in a conversation context.
    /// </remarks>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Role
    {
        /// <summary>
        /// Represents the user or human participant.
        /// </summary>
        User,
        
        /// <summary>
        /// Represents the assistant or AI participant.
        /// </summary>
        Assistant
    }
}
