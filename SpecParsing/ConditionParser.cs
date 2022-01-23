using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ABRISPlanner.SpecParsing
{
    public class ConditionParser : JsonConverter<List<RuleCondition>>
    {
        public override List<RuleCondition> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType switch
        {
            JsonTokenType.StartObject => new() { JsonSerializer.Deserialize<RuleCondition>(ref reader, options) },
            JsonTokenType.StartArray => ParseConditionArray(ref reader,options),
            _ => throw new ArgumentException("Cannot parse condition")
        };

        private List<RuleCondition> ParseConditionArray(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            List<RuleCondition> conditions = new();
            while (true)
            {
                reader.Read();
                if (reader.TokenType == JsonTokenType.EndArray) break;
                conditions.Add(JsonSerializer.Deserialize<RuleCondition>(ref reader, options));
                
            }
            return conditions;
        }

        public override void Write(Utf8JsonWriter writer, List<RuleCondition> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
