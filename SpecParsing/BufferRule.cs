using System.Text.Json.Serialization;

namespace ABRISPlanner.SpecParsing
{
    public class BufferRule : BasicRule
    {
        [JsonPropertyName("style")]
        public string Style { get; set; }
        [JsonPropertyName("bufferSize")]
        public double BufferSize { get; set; }
    }
}