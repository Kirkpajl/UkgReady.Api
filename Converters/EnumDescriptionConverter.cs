using System;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using UkgReady.Api.Extensions;

namespace UkgReady.Api.Converters
{
    internal class EnumDescriptionConverter : JsonConverter<Enum>
    {
        public override bool CanConvert(Type objectType)
        {
            var enumType = Nullable.GetUnderlyingType(objectType) ?? objectType;
            return enumType.IsEnum;
        }

        public override Enum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string description = reader.GetString();

            if (description is null) return default;

            foreach (var field in typeToConvert.GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (Enum)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
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
