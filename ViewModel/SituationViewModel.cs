using ABRISPlanner.Model;
using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using ABRISPlanner.SpecParsing;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace ABRISPlanner.ViewModel
{
    public class SituationViewModel
    {
        private const string ServerSpec = @"Specs/rotorheads.serverspec.json";
        readonly private Situation Situation;
        public SituationViewModel(Situation situation)
        {
            Situation = situation;
        }
        internal void Update()
        {
            var options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true
            };

            var specText = File.ReadAllText(ServerSpec);
            var spec = JsonSerializer.Deserialize<ServerSpec>(specText, options);
            Dictionary<string, JsonDocument> features = new(spec.DataSources.Select((x) => new KeyValuePair<string, JsonDocument>(x.Key, ParseJson(spec.ServerUrl, x.Value))));
            Situation.Symbols = spec.PointRules.SelectMany(rule => ParsePoints(rule, features)).ToList();
        }
        JsonDocument ParseJson(string url, string file)
        {
            System.Net.WebClient wc = new System.Net.WebClient();
 
            string json = wc.DownloadString(url + file);
            return JsonDocument.Parse(json);
        }
        IEnumerable<SymbolSituationObject> ParsePoints(PointRule rule, Dictionary<string, JsonDocument> featureMap)
        {
            foreach(var feature in GetFeatures(rule,featureMap))
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

        IEnumerable<JsonElement> GetFeatures(BasicRule rule, Dictionary<string, JsonDocument> featureMap) =>
            rule.Sources.Select(source => featureMap[source])
                .Select(map => map.RootElement.GetProperty("features"))
                .SelectMany(property => property.EnumerateArray())
                .Where(ftr => rule.Condition?.Match(ftr) ?? true);
        PointF GetLocation(JsonElement feature)
        {
            var coordinates = feature.GetProperty("geometry").GetProperty("coordinates").EnumerateArray().Select(xy => xy.GetDouble()).ToArray();
            return new((float)coordinates[0], (float)coordinates[1]);
        }
    }
}
