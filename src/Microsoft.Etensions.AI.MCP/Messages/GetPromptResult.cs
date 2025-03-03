using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Extensions.AI.MCP.Models;

namespace Microsoft.Extensions.AI.MCP.Messages
{
    /// <summary>
    /// The server's response to a prompts/get request from the client.
    /// </summary>
    public class GetPromptResult : IServerResult
    {
        /// <summary>
        /// Gets or sets an optional description for the prompt.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the messages that make up the prompt.
        /// </summary>
        [JsonPropertyName("messages")]
        public List<PromptMessage> Messages { get; set; } = new List<PromptMessage>();
        
        /// <summary>
        /// The prompt property for compatibility with existing code.
        /// </summary>
        public Prompt Prompt 
        { 
            get => new Prompt 
            { 
                Description = Description ?? string.Empty,
                Messages = Messages
            };
            set
            {
                Description = value.Description;
                Messages = value.Messages;
            }
        }
    }
}