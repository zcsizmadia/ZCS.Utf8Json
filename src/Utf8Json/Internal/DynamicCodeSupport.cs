using System;

namespace Utf8Json.Internal
{
    /// <summary>
    /// Whether the runtime can generate code at run time (Reflection.Emit / DynamicMethod).
    /// False under native AOT; the dynamic resolvers then decline to build formatters instead of crashing.
    /// </summary>
    internal static class DynamicCodeSupport
    {
        // Lets the test suite exercise the non-emit fallbacks on a JIT runtime (set through reflection). Never set in production.
        static bool simulateUnsupported = false;

        public static bool IsSupported
        {
            get
            {
                if (simulateUnsupported) return false;
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

        /// <summary>
        /// MethodInfo.Invoke that rethrows the target's own exception instead of TargetInvocationException,
        /// so reflection-based fallbacks surface the same exceptions as the emitted code paths.
        /// </summary>
        public static object Invoke(System.Reflection.MethodInfo method, object target, object[] args)
        {
            try
            {
                return method.Invoke(target, args);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                if (ex.InnerException == null) throw;
#if NETSTANDARD || NETCOREAPP
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw; // unreachable
#else
                throw ex.InnerException;
#endif
            }
        }
    }
}
