using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebJet.Entertainment.Poc
{
    public class DecimalConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                return reader.TokenType switch
                {
                    JsonTokenType.String when decimal.TryParse(reader.GetString(), out var d) => d,
                    JsonTokenType.Number => reader.GetDecimal(),
                    _ => throw new JsonException($"Cannot convert {reader.TokenType} to decimal")
                };
            }
            catch (Exception ex)
            {
                throw new JsonException("Error converting value to decimal", ex);
            }
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options) =>
            writer.WriteNumberValue(value);
    }
}