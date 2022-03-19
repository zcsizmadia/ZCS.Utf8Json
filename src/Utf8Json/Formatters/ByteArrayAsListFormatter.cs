using System;
using System.Collections.Generic;
using System.Text;

namespace Utf8Json.Formatters
{
    // MEMO: Not recommended. should write/read base64 directly like corefxlab/System.Binary.Base64
    // https://github.com/dotnet/runtime/issues/30456
    public sealed class ByteArrayAsListFormatter : IJsonFormatter<byte[]>
    {
        public static readonly IJsonFormatter<byte[]> Default = new ByteArrayAsListFormatter();

        public void Serialize(ref JsonWriter writer, byte[] value, IJsonFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNull(); return; }

            new ArrayFormatter<byte>().Serialize(ref writer, value, formatterResolver);
        }

        public byte[] Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull()) return null;

            if (reader.GetCurrentJsonToken() == JsonToken.BeginArray)
            {
                return new ArrayFormatter<byte>().Deserialize(ref reader, formatterResolver);
            }

            return ByteArrayFormatter.Default.Deserialize(ref reader, formatterResolver);
        }
    }
}
