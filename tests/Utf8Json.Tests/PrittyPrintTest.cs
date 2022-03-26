using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Utf8Json.Tests
{
    public class PrittyPrintTest
    {

        [Theory]
        [InlineData(double.MaxValue, "1.7976931348623157E+308")]
        [InlineData(double.MinValue, "-1.7976931348623157E+308")]
        [InlineData(double.NaN, "NaN")]
        [InlineData(double.NegativeInfinity, "-Infinity")]
        [InlineData(double.PositiveInfinity, "Infinity")]
        public void Double(double value, string expected)
        {
            var bytes = JsonSerializer.Serialize(value);
            var printed = JsonSerializer.PrettyPrint(bytes);

            printed.Is(expected);
        }

        [Theory]
        [InlineData(long.MinValue, "-9223372036854775808")]
        [InlineData(long.MaxValue, "9223372036854775807")]
        public void Long(long value, string expected)
        {
            var bytes = JsonSerializer.Serialize(value);
            var printed = JsonSerializer.PrettyPrint(bytes);

            printed.Is(expected);
        }

        [Fact]
        public void Ulong()
        {
            var value = ulong.MaxValue;

            var bytes = JsonSerializer.Serialize(value);
            var printed = JsonSerializer.PrettyPrint(bytes);

            printed.Is("18446744073709551615");
        }
    }
}
