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

        [Fact]
        public void Double()
        {
            var value = double.MaxValue;

            var bytes = JsonSerializer.Serialize(value);
            var printed = JsonSerializer.PrettyPrint(bytes);

            printed.Is("1.7976931348623157E+308");
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
