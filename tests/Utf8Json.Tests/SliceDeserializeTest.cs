using System;
using System.Buffers;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Utf8Json.Tests
{
    public class SliceDeserializeTest
    {
        public class AB
        {
            public int A { get; set; }
            public string B { get; set; }
        }

        static byte[] Bytes(string s) => Encoding.UTF8.GetBytes(s);

        [Fact]
        public void NumberAtEndOfSliceDoesNotReadPastCount()
        {
            var buf = Bytes("12345");

            Assert.Equal(12, JsonSerializer.Deserialize<int>(buf, 0, 2));
            Assert.Equal(12L, JsonSerializer.Deserialize<long>(buf, 0, 2));
            Assert.Equal(12u, JsonSerializer.Deserialize<uint>(buf, 0, 2));
            Assert.Equal(12UL, JsonSerializer.Deserialize<ulong>(buf, 0, 2));
            Assert.Equal(-12, JsonSerializer.Deserialize<int>(Bytes("-12345"), 0, 3));
            Assert.Equal(1.5, JsonSerializer.Deserialize<double>(Bytes("1.599"), 0, 3));
            Assert.Equal(1.5f, JsonSerializer.Deserialize<float>(Bytes("1.599"), 0, 3));
        }

        [Fact]
        public void NumberInMiddleOfBuffer()
        {
            var buf = Bytes("xx12345");
            Assert.Equal(12, JsonSerializer.Deserialize<int>(buf, 2, 2));
            Assert.Equal(12, JsonSerializer.Deserialize<int>(new ArraySegment<byte>(buf, 2, 2)));
        }

        [Fact]
        public void ObjectInSlice()
        {
            var buf = Bytes("{\"A\":1,\"B\":\"x\"}{\"A\":999,\"B\":\"stale\"}");
            var v = JsonSerializer.Deserialize<AB>(buf, 0, 15);
            Assert.Equal(1, v.A);
            Assert.Equal("x", v.B);
        }

        [Fact]
        public void TruncatedObjectThrowsInsteadOfReadingPastCount()
        {
            var buf = Bytes("{\"A\":1234}");
            Assert.Throws<JsonParsingException>(() => JsonSerializer.Deserialize<AB>(buf, 0, 7));
        }

        [Fact]
        public void TruncatedLiteralsThrow()
        {
            Assert.Throws<JsonParsingException>(() => JsonSerializer.Deserialize<bool>(Bytes("true"), 0, 3));
            Assert.Throws<JsonParsingException>(() => JsonSerializer.Deserialize<bool>(Bytes("false"), 0, 4));
            Assert.Throws<JsonParsingException>(() => JsonSerializer.Deserialize<string>(Bytes("null"), 0, 3));
            Assert.Throws<JsonParsingException>(() => JsonSerializer.Deserialize<bool>(Bytes("true"), 0, 0));
        }

        [Fact]
        public void BomIsSkippedInsideSlice()
        {
            var buf = new byte[] { 0xEF, 0xBB, 0xBF, (byte)'7', (byte)'8' };
            Assert.Equal(7, JsonSerializer.Deserialize<int>(buf, 0, 4));
        }

        [Fact]
        public void BomProbeDoesNotReadPastBuffer()
        {
            var buf = new byte[] { (byte)'1', (byte)'2', 0xEF };

            // used to throw IndexOutOfRangeException from the constructor
            new JsonReader(buf, buf.Length);
            new JsonReader(buf, 2);
            new JsonReader(buf, 1);
            new JsonReader(buf, 2, 1);

            Assert.Throws<JsonParsingException>(() => JsonSerializer.Deserialize<int>(buf, buf.Length));
        }

        [Fact]
        public void InvalidRangeThrows()
        {
            var buf = Bytes("123");
            Assert.Throws<ArgumentNullException>(() => new JsonReader(null, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonReader(buf, -1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonReader(buf, 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonReader(buf, 2, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => new JsonReader(buf, 4, 0));
        }

        [Fact]
        public void ReadOnlyMemoryArrayBacked()
        {
            var buf = Bytes("xx12345");
            Assert.Equal(12, JsonSerializer.Deserialize<int>(new ReadOnlyMemory<byte>(buf, 2, 2)));
            Assert.Equal(12, JsonSerializer.Deserialize<int>(new Memory<byte>(buf, 2, 2)));
        }

        [Fact]
        public void ReadOnlyMemoryNotArrayBacked()
        {
            using var owner = new NonArrayMemoryManager(Bytes("xx12345"));
            Assert.Equal(12, JsonSerializer.Deserialize<int>(owner.Memory.Slice(2, 2)));
        }

        [Fact]
        public void StreamNumberUsesStreamLength()
        {
            var ms = new MemoryStream(new byte[16], 0, 16, true, true);
            ms.Write(Bytes("12"), 0, 2);
            ms.SetLength(2);
            ms.Position = 0;
            Assert.Equal(12, JsonSerializer.Deserialize<int>(ms));
        }

        [Fact]
        public async Task DeserializeAsyncNumber()
        {
            // fill the pool buffer with digits first, then read a shorter number
            Assert.Equal(123456789, await JsonSerializer.DeserializeAsync<int>(new MemoryStream(Bytes("123456789"))));
            Assert.Equal(12, await JsonSerializer.DeserializeAsync<int>(new MemoryStream(Bytes("12"))));
        }

        [Fact]
        public void TimeSpanDaysDoNotPickUpStalePoolBytes()
        {
            var big = new TimeSpan(123, 1, 0, 0);
            var small = new TimeSpan(4, 1, 0, 0);
            Assert.Equal(big, JsonSerializer.Deserialize<TimeSpan>(JsonSerializer.Serialize(big)));
            Assert.Equal(small, JsonSerializer.Deserialize<TimeSpan>(JsonSerializer.Serialize(small)));
        }

        sealed class NonArrayMemoryManager : MemoryManager<byte>
        {
            readonly byte[] data;

            public NonArrayMemoryManager(byte[] data) { this.data = data; }

            public override Span<byte> GetSpan() => data;

            public override MemoryHandle Pin(int elementIndex = 0) => throw new NotSupportedException();

            public override void Unpin() { }

            // TryGetArray is not overridden, so MemoryMarshal.TryGetArray returns false.
            protected override void Dispose(bool disposing) { }
        }
    }
}
