using System.Net.Mime;
using Microsoft.Extensions.ModelContextProtocol.Annotations;

namespace Microsoft.Extensions.ModelContextProtocol.Protocol
{
    // ...existing code...

    /// <summary>
    /// Audio provided to or from an LLM.
    /// (Based on schema.ts.txt: Updated to support audio content with properties "Data" and "MimeType".)
    /// </summary>
    public class AudioContent : Annotated
    {
        // The "type" property is fixed to "audio" per draft schema.
        public string Type => "audio";
        public string Data { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
    }

    // ...existing code...
}
