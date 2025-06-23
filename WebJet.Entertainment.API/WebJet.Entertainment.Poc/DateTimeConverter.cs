using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebJet.Entertainment.Poc
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private const string Format = "dd MMM yyyy";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();
            return DateTime.ParseExact(s, Format, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format));
        }
    }
}