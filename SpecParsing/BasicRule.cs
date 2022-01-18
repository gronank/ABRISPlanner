using System.Collections.Generic;
using System.Drawing;
using System.Text.Json.Serialization;

namespace ABRISPlanner.SpecParsing
{
    public class BasicRule
    {
        [JsonPropertyName("sources")]
        public List<string> Sources { get; set; }
        [JsonPropertyName("disable")]
        public string Disable { get; set; }
        [JsonPropertyName("condition")]
        public RuleCondition Condition { get; set; }
        [JsonPropertyName("name")]
        public RuleCondition Name { get; set; }
        public Color Color { get; set; }
        [JsonPropertyName("color")]
        public string ColorString { set => Color = Color.FromName(value); }
    }
}