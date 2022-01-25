using ABRISPlanner.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ABRISPlanner.SpecParsing
{
    class ServerSpec
    {
        private Dictionary<string, JsonDocument> FeatureMap_;
        private Dictionary<string, JsonDocument> FeatureMap { get { FeatureMap_ ??= new(DataSources.Select((x) => new KeyValuePair<string, JsonDocument>(x.Key, ParseJson(ServerUrl, x.Value)))); return FeatureMap_; } }
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

        public static ServerSpec Open(string path)
        {
            var options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
            };
            options.Converters.Add(new ConditionParser());
            var specText = File.ReadAllText(path);
            return JsonSerializer.Deserialize<ServerSpec>(specText, options);
        }

        internal List<SymbolSituationObject> ReadSymbols() => PointRules.SelectMany(ParsePoints).ToList();
        internal List<ZoneSituationObject> ReadZones() => BufferRules.SelectMany(ParseZones).ToList();

        private static JsonDocument ParseJson(string url, string file)
        {
            System.Net.WebClient wc = new System.Net.WebClient();

            string json = wc.DownloadString(url + file);
            return JsonDocument.Parse(json);
        }
        private IEnumerable<SymbolSituationObject> ParsePoints(PointRule rule)
        {
            foreach (JsonElement feature in GetFeatures(rule, FeatureMap))
            {
                yield return new()
                {
                    Location = GetLocation(feature),
                    Name = rule.Name.Parse(feature),
                    SymbolType = rule.SymbolType,
                    Disable = rule.Disable
                };
            }
        }
        private IEnumerable<ZoneSituationObject> ParseZones(BufferRule rule)
        {
            var points = GetFeatures(rule, FeatureMap).Select(GetLocation);
            foreach(var line in ParseGeometry.ComputeBuffer(points,rule.BufferSize))
            {
                yield return new()
                {
                    Buffer = line,
                    Style = rule.Style,
                    Color = rule.Color
                };
            }
        }

        private IEnumerable<JsonElement> GetFeatures(BasicRule rule, Dictionary<string, JsonDocument> featureMap) =>
            rule.Sources.Select(source => featureMap[source])
                .Select(map => map.RootElement.GetProperty("features"))
                .SelectMany(property => property.EnumerateArray())
                .Where(rule.Match);
        private static PointF GetLocation(JsonElement feature)
        {
            var coordinates = feature.GetProperty("geometry").GetProperty("coordinates").EnumerateArray().Select(xy => xy.GetDouble()).ToArray();
            return new((float)coordinates[0], (float)coordinates[1]);
        }
    }
}
