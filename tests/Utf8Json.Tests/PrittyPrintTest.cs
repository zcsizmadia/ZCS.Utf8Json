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

        [Fact]
        public void Long()
        {
            var value = long.MinValue;

            var bytes = JsonSerializer.Serialize(value);
            var printed = JsonSerializer.PrettyPrint(bytes);

            printed.Is("-9223372036854775808");
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
