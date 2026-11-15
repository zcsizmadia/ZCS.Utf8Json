using System.Globalization;
using System.Text;

namespace Utf8Json.CodeGenerator.Generator
{
    /// <summary>
    /// Minimal line-oriented text builder for the generated source.
    /// Always emits "\r\n" so the output does not depend on the OS or on how the generator's own sources were checked out.
    /// </summary>
    internal sealed class CodeWriter
    {
        const string NewLine = "\r\n";

        readonly StringBuilder sb = new StringBuilder();

        /// <summary>Appends <paramref name="text"/> followed by a newline.</summary>
        public void Line(string text = "")
        {
            sb.Append(text).Append(NewLine);
        }

        /// <summary>Appends <paramref name="text"/> without a trailing newline.</summary>
        public void Text(string text)
        {
            sb.Append(text);
        }

        public static string N(int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }
}
