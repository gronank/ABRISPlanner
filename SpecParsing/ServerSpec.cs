using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ABRISPlanner.SpecParsing
{
    class ServerSpec
    {
        [JsonPropertyName("serverUrl")]
        public string ServerUrl { get; set; }
        [JsonPropertyName("dataSources")]
        public Dictionary<string,string> DataSources { get; set; }
        [JsonPropertyName("points")]
        public List<PointRule> PointRules { get; set; }
        [JsonPropertyName("bufferZones")]
        public List<BufferRule> BufferRules { get; set; }
        [JsonPropertyName("lines")]
        public List<LineRule> LineRules { get; set; }
    }
}
