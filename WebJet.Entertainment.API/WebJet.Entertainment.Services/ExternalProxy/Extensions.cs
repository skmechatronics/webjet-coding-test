using System.Text.Json;

namespace WebJet.Entertainment.Services.ExternalProxy
{
    internal static class Extensions
    {
        public static string GetReaderValue(this Utf8JsonReader reader)
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
