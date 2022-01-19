﻿using ABRISPlanner.Model;
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
                AllowTrailingCommas = true
            };
            var specText = File.ReadAllText(ServerSpec);
            var spec = JsonSerializer.Deserialize<ServerSpec>(specText, options);
            Situation.Symbols=spec.DataSources.SelectMany(x => ParseFeatures(x, spec)).Select(element=>ParsePoint(element.Key, element.Element, spec.PointRules)).Where(x=>x!=null).ToList();
            //Dictionary<string, List<JsonElement>> features = new(spec.DataSources.Select((x) => new KeyValuePair<string, List<JsonElement>>(x.Key, ParseJson(spec.ServerUrl, x.Value).RootElement.GetProperty("features").EnumerateArray().ToList())));
            //Situation.Symbols = spec.PointRules.SelectMany(rule => ParsePoints(rule, features)).ToList();
            Changed("Situation");
        }

        private IEnumerable<(string Key,JsonElement Element)> ParseFeatures(KeyValuePair<string, string> x, ServerSpec spec)
        {
            foreach(var element in ParseJson(spec.ServerUrl, x.Value).RootElement.GetProperty("features").EnumerateArray())
            {
                yield return (x.Key, element);
            }
        }

        JsonDocument ParseJson(string url, string file)
        {
            System.Net.WebClient wc = new System.Net.WebClient();
 
            string json = wc.DownloadString(url + file);
            return JsonDocument.Parse(json);
        }
        SymbolSituationObject ParsePoint(string Key, JsonElement element, List<PointRule> rules)
        {
            var selectedRule = rules.Where(rule => rule.Sources.Contains(Key)).FirstOrDefault(rule => rule.Condition.Match(element));
            if (selectedRule == null)
                return null;
            return new()
            {
                Location = GetLocation(element),
                Name = selectedRule.Name.Parse(element),
                SymbolType = selectedRule.SymbolType,
                Disable = selectedRule.Disable
            };
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
