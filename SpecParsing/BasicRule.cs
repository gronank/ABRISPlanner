using System.Collections.Generic;
using System.Drawing;
using System.Text.Json.Serialization;
using System.Linq;
using System.Text.Json;

namespace ABRISPlanner.SpecParsing
{
    public class BasicRule
    {
        [JsonPropertyName("sources")]
        public List<string> Sources { get; set; }
        [JsonPropertyName("disable")]
        public string Disable { get; set; }
        [JsonPropertyName("condition")]
        public List<RuleCondition> Conditions { get; set; }
        [JsonPropertyName("name")]
        public RuleCondition Name { get; set; }
        public Color Color { get; set; }
        [JsonPropertyName("color")]
        public string ColorString { set => Color = Color.FromName(value); }
        public bool Match(JsonElement ftr) => !Conditions.Any(c => !c.Match(ftr));
    }
}