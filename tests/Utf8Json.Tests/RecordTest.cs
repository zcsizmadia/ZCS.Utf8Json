using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json.Resolvers;
using Xunit;

namespace Utf8Json.Tests
{
    public class RecordTest
    {
        T Convert<T>(T value) => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value));

        record classRecord(int i, string s);
        record struct structRecord(int i, string s);

        [Fact]
        public void Serialize()
        {
            var a = new classRecord(10, "abc");
            var b = new structRecord(10, "abc");

            JsonSerializer.SetDefaultResolver(StandardResolver.AllowPrivate);

            var json_a = JsonSerializer.ToJsonString(a);
            var json_b = JsonSerializer.ToJsonString(b);

            json_a.Is(json_b);
            json_a.Is(@"{""i"":10,""s"":""abc""}");
        }


        [Fact]
        public void Deserialize()
        {
            var a = new classRecord(10, "abc");
            var b = new structRecord(10, "abc");

            JsonSerializer.SetDefaultResolver(StandardResolver.AllowPrivate);

            var convert_a = Convert(a);
            var convert_b = Convert(b);

            convert_a.Is(a);
            convert_b.Is(b);
        }
    }
}
