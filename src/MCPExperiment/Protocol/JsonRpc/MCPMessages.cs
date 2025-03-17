using System;
using System.Collections.Generic;

namespace MCPExperiment.Protocol.JsonRpc
{
    // ...existing code...

    /// <summary>
    /// Represents a message used in sampling.
    /// (Based on schema.ts.txt: Updated to support audio content by making Content a generic object.)
    /// </summary>
    public interface SamplingMessage
    {
        string Role { get; set; }
        // Now supports text, image, or audio content.
        object Content { get; set; }
    }

    /// <summary>
    /// The client's response to a sampling/createMessage request.
    /// (Based on schema.ts.txt: Added model and stopReason properties.)
    /// </summary>
    public class CreateMessageResult : SamplingMessage
    {
        public string Role { get; set; } = string.Empty;
        public object Content { get; set; } = null!;
        public string Model { get; set; } = string.Empty;
        public string? StopReason { get; set; }
    }

    // ...existing code...
}
