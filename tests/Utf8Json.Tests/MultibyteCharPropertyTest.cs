using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Utf8Json.Tests
{
    public class MultibyteCharPropertyTest
    {
        public class データ
        {
            public int A { get; set; }

            public int B { get; set; }

            public int にほんご { get; set; }

            public int 简体字 { get; set; }

            public int 훈민정음 { get; set; }
        }

        [Fact]
        public void ConvertMultibyteCharProperty()
        {
            var data = new データ
            {
                A = 1,
                B = 2,
                にほんご = 3,
                简体字 = 4,
                훈민정음 = 5,
            };

            JsonSerializer.ToJsonString(data).Is(@"{""A"":1,""B"":2,""にほんご"":3,""简体字"":4,""훈민정음"":5}");

            byte[] bytes = JsonSerializer.Serialize(data);
            var a = JsonSerializer.Deserialize<データ>(bytes);
            a.A.Is(data.A);
            a.B.Is(data.B);
            a.にほんご.Is(data.にほんご);
            a.简体字.Is(data.简体字);
            a.훈민정음.Is(data.훈민정음);
        }
    }
}
