using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ABRISPlanner.SpecParsing
{
    public class PointRule: BasicRule
    {
        [JsonPropertyName("symbolType")]
        public string SymbolType { get; set; }
    }
}