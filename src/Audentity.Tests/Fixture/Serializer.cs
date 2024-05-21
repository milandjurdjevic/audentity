using System.Text.Json;
using System.Text.Json.Serialization;

namespace Audentity.Tests.Fixture;

public static class Serializer
{
    public static string Serialize<T>(T value)
    {
        JsonSerializerOptions options = new();
        options.WriteIndented = true;
        options.Converters.Add(new TypeConverter());
        return JsonSerializer.Serialize(value, options);
    }

    private class TypeConverter : JsonConverter<Type>
    {
        public override Type? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return null;
        }

        public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Name);
        }
    }
}