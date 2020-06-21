using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sag.Data.Common.Query.Internal
{
    class TypeToJsonConverter:JsonConverter<Type>
    {
        public override Type Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
                if (reader.TokenType == JsonTokenType.String)
                {
                    return Type.GetType(reader.GetString());
                }
            return typeof(object);
        }
    
        public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.FullName);
        }
    }

}

 