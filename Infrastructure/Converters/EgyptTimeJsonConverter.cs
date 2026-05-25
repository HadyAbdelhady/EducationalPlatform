using System.Text.Json;
using System.Text.Json.Serialization;
using Domain;

namespace Infrastructure.Converters
{
    public class EgyptTimeJsonConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.GetDateTimeOffset();

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
            => writer.WriteStringValue(EgyptTime.ToEgypt(value));
    }

    public class NullableEgyptTimeJsonConverter : JsonConverter<DateTimeOffset?>
    {
        public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.TokenType == JsonTokenType.Null ? null : reader.GetDateTimeOffset();

        public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteStringValue(EgyptTime.ToEgypt(value.Value));
            else
                writer.WriteNullValue();
        }
    }
}
