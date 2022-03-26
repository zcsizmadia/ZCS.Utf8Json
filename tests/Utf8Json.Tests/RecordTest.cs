using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json.Resolvers;
using Xunit;

namespace Utf8Json.Tests
{
    public record ClassRecord(int Int, string Str);
    public record struct StructRecord(int Int, string Str);

    public class RecordTest
    {
        static T Convert<T>(T value) => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value, resolver), resolver);

        static readonly IJsonFormatterResolver resolver = CompositeResolver.Create(StandardResolver.AllowPrivateCamelCase);

        [Fact]
        public void Serialize()
        {
            var a = new ClassRecord(10, "abc");
            var b = new StructRecord(10, "abc");

            var json_a = JsonSerializer.ToJsonString(a, resolver);
            var json_b = JsonSerializer.ToJsonString(b, resolver);

            json_a.Is(json_b);
            json_a.Is(@"{""int"":10,""str"":""abc""}");
        }

        [Fact]
        public void Deserialize()
        {
            var a = new ClassRecord(10, "abc");
            var b = new StructRecord(10, "abc");

            var convert_a = Convert(a);
            var convert_b = Convert(b);

            convert_a.Is(a);
            convert_b.Is(b);
        }
    }
}
