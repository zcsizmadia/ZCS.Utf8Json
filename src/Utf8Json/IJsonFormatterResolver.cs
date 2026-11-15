using System;
using System.Reflection;

namespace Utf8Json
{
    public interface IJsonFormatterResolver
    {
        IJsonFormatter<T> GetFormatter<T>();
    }

    public static class JsonFormatterResolverExtensions
    {
        public static IJsonFormatter<T> GetFormatterWithVerify<T>(this IJsonFormatterResolver resolver)
        {
            IJsonFormatter<T> formatter;
            try
            {
                formatter = resolver.GetFormatter<T>();
            }
            catch (TypeInitializationException ex)
            {
                Exception inner = ex;
                while (inner.InnerException != null)
                {
                    inner = inner.InnerException;
                }

                throw inner;
            }

            if (formatter == null)
            {
                var message = typeof(T).FullName + " is not registered in this resolver. resolver:" + resolver.GetType().Name;
                if (!Utf8Json.Internal.DynamicCodeSupport.IsSupported)
                {
                    message += ". " + Utf8Json.Internal.DynamicCodeSupport.NotSupportedHint;
                }
                throw new FormatterNotRegisteredException(message);
            }

            return formatter;
        }

        [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(Utf8Json.Internal.DynamicCodeSupport.RequiresDynamicCodeMessage)]
        public static object GetFormatterDynamic(this IJsonFormatterResolver resolver, Type type)
        {
            var methodInfo = typeof(IJsonFormatterResolver).GetRuntimeMethod("GetFormatter", Type.EmptyTypes);

            var formatter = methodInfo.MakeGenericMethod(type).Invoke(resolver, null);
            return formatter;
        }
    }

    public class FormatterNotRegisteredException : Exception
    {
        public FormatterNotRegisteredException(string message) : base(message)
        {
        }
    }
}