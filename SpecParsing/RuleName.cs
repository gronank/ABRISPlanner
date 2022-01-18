using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ABRISPlanner.SpecParsing
{
    public class RuleName
    {
        [JsonPropertyName("property")]
        public string Property { get; set; }
        [JsonPropertyName("pattern")]
        public string Pattern { get; set; }
        
    }
}