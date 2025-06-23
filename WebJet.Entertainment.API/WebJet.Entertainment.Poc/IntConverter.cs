using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebJet.Entertainment.Poc
{
    public class IntConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                return reader.TokenType switch
                {
                    JsonTokenType.String when int.TryParse(reader.GetString(), out var i) => i,
                    JsonTokenType.Number => reader.GetInt32(),
                    _ => throw new JsonException($"Cannot convert token of type '{reader.TokenType}' to int. Value: '{GetReaderValue(reader)}'")
                };
            }
            catch (Exception ex)
            {
                throw new JsonException("Error converting value to int", ex);
            }
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options) =>
            writer.WriteNumberValue(value);

        private string GetReaderValue(Utf8JsonReader reader)
        {
            return reader.TokenType switch
            {
                JsonTokenType.String => reader.GetString() ?? "String was NULL",
                JsonTokenType.Number => reader.TryGetInt64(out var val) ? val.ToString() : "[unreadable number]",
                JsonTokenType.True => "true",
                JsonTokenType.False => "false",
                JsonTokenType.Null => "null",
                _ => "[unknown or unsupported value]"
            };
        }


    }
}