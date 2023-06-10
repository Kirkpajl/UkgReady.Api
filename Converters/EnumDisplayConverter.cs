using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using UkgReady.Api.Extensions;

namespace UkgReady.Api.Converters
{
    internal class EnumDisplayConverter : JsonConverter<Enum>
    {
        public override bool CanConvert(Type objectType)
        {
            var enumType = Nullable.GetUnderlyingType(objectType) ?? objectType;
            return enumType.IsEnum;
        }

        public override Enum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string name = reader.GetString();

            if (name is null) return default;

            foreach (var field in typeToConvert.GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) is DisplayAttribute attribute)
                {
                    if (attribute.Name == name)
                        return (Enum)field.GetValue(null);
                }
                else
                {
                    if (field.Name == name)
                        return (Enum)field.GetValue(null);
                }
            }

            throw new ArgumentException("Not found.");
        }

        public override void Write(Utf8JsonWriter writer, Enum value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.GetDescription() ?? value.ToString());
        }
    }
}
