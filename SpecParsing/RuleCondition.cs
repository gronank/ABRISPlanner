using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace ABRISPlanner.SpecParsing
{
    public class RuleCondition
    { 
        [JsonPropertyName("property")]
        public string Property { get; set; }
        private Regex Pattern; 
        [JsonPropertyName("pattern")]
        public string SetPattern { set=>Pattern = new(value); }
        [JsonPropertyName("inverted")]
        public bool Inverted { get; set; } = false;
        [JsonPropertyName("replace")]
        public string SetReplace { set => Replace = Regex.Replace(value, "\\\\(\\d)","$$1"); }
        public string Replace { get; set; }

        internal string Parse(JsonElement ftr) => Pattern?.Replace(GetValue(ftr), Replace)??"";  
        internal bool Match(JsonElement ftr) => Inverted != (Pattern?.IsMatch(GetValue(ftr))??!Inverted);

        private string GetValue(JsonElement ftr) => ftr.GetProperty("properties").GetProperty(Property).GetString();
    }
}