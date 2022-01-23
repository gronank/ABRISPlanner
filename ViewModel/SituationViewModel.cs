using ABRISPlanner.Model;
using System.Text.Json;
using ABRISPlanner.SpecParsing;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using TacticalSimWpf.ViewModel;

namespace ABRISPlanner.ViewModel
{
    public class SituationViewModel : Changeable
    {
        private const string ServerSpec = @"Specs/rotorheads.serverspec.json";
        readonly public Situation Situation;
        public SituationViewModel(Situation situation)
        {
            Situation = situation;
        }
        internal void Update()
        {
            
            var options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
            };
            options.Converters.Add(new ConditionParser());
            var specText = File.ReadAllText(ServerSpec);
            var spec = JsonSerializer.Deserialize<ServerSpec>(specText, options);
            //var situationObjects = spec.DataSources.SelectMany(x => ParseFeatures(x, spec)).Select(element => ParseSituationObject(element.Key, element.Element, spec)).Where(x => x != null).ToList();
            //Situation.Symbols= situationObjects.OfType<SymbolSituationObject>().ToList();
            Dictionary<string, JsonDocument> features = new(spec.DataSources.Select((x) => new KeyValuePair<string, JsonDocument>(x.Key, ParseJson(spec.ServerUrl, x.Value))));
            Situation.Symbols = spec.PointRules.SelectMany(rule => ParsePoints(rule, features)).ToList();
            Changed("Situation");
        }

        //private IEnumerable<(string Key,JsonElement Element)> ParseFeatures(KeyValuePair<string, string> x, ServerSpec spec)
        //{
        //    foreach(var element in ParseJson(spec.ServerUrl, x.Value).RootElement.GetProperty("features").EnumerateArray())
        //    {
        //        yield return (x.Key, element);
        //    }
        //}

        private static JsonDocument ParseJson(string url, string file)
        {
            System.Net.WebClient wc = new System.Net.WebClient();
 
            string json = wc.DownloadString(url + file);
            return JsonDocument.Parse(json);
        }
        //private static SituationObject ParseSituationObject(string Key, JsonElement element, ServerSpec spec)
        //{
        //    var selectedRule = spec.PointRules.Where(rule => rule.Sources.Contains(Key)).FirstOrDefault(rule => rule.Match(element));
        //    if (selectedRule != null)
        //    {
        //        return new SymbolSituationObject()
        //        {
        //            Location = GetLocation(element),
        //            Name = selectedRule.Name.Parse(element),
        //            SymbolType = selectedRule.SymbolType,
        //            Disable = selectedRule.Disable
        //        };
        //    }
        //    return null;
        //}
        private IEnumerable<SymbolSituationObject> ParsePoints(PointRule rule, Dictionary<string, JsonDocument> featureMap)
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
                .Where(rule.Match);
        private static PointF GetLocation(JsonElement feature)
        {
            var coordinates = feature.GetProperty("geometry").GetProperty("coordinates").EnumerateArray().Select(xy => xy.GetDouble()).ToArray();
            return new((float)coordinates[0], (float)coordinates[1]);
        }
    }
}
