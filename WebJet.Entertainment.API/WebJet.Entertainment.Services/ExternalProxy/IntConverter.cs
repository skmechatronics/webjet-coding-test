using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebJet.Entertainment.Services.ExternalProxy
{
    public class IntConverter : JsonConverter<int>
    {
        // For numbers such as "423,122"
        private const string Comma = ",";

        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                return reader.TokenType switch
                {
                    JsonTokenType.String when int.TryParse(reader.GetString().Replace(Comma, string.Empty), out var i) => i,
                    JsonTokenType.Number => reader.GetInt32(),
                    _ => throw new JsonException($"Cannot convert token of type '{reader.TokenType}' to int. Value: '{reader.GetReaderValue()}'")
                };
            }
            catch (Exception ex)
            {
                throw new JsonException("Error converting value to int", ex);
            }
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options) =>
            writer.WriteNumberValue(value);

    }
}