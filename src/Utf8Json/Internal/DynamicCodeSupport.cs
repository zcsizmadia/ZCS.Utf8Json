using System;

namespace Utf8Json.Internal
{
    /// <summary>
    /// Whether the runtime can generate code at run time (Reflection.Emit / DynamicMethod).
    /// False under native AOT; the dynamic resolvers then decline to build formatters instead of crashing.
    /// </summary>
    internal static class DynamicCodeSupport
    {
        public static bool IsSupported
        {
            get
            {
#if NETCOREAPP
                return System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported;
#else
                // netstandard and Unity builds cannot detect it; assume supported (the previous behaviour).
                return true;
#endif
            }
        }

        public const string NotSupportedHint =
            "Dynamic code generation is not supported in this runtime (for example native AOT), so formatters cannot be generated at run time. " +
            "Generate formatters ahead of time with Utf8Json.UniversalCodeGenerator and register the generated resolver.";

        public static NotSupportedException CreateNotSupportedException(string what)
        {
            return new NotSupportedException(what + " requires dynamic code generation. " + NotSupportedHint);
        }
    }
}
