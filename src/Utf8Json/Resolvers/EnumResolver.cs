using System;
using Utf8Json.Internal.Emit;
using Utf8Json.Internal;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using Utf8Json.Formatters;
using Utf8Json.Resolvers.Internal;

namespace Utf8Json.Resolvers
{
    public static class EnumResolver
    {
        /// <summary>Serialize as Name.</summary>
        public static readonly IJsonFormatterResolver Default = EnumDefaultResolver.Instance;
        /// <summary>Serialize as Value.</summary>
        public static readonly IJsonFormatterResolver UnderlyingValue = EnumUnderlyingValueResolver.Instance;
    }
}

namespace Utf8Json.Resolvers.Internal
{
    internal sealed class EnumDefaultResolver : IJsonFormatterResolver
    {
        public static readonly IJsonFormatterResolver Instance = new EnumDefaultResolver();

        EnumDefaultResolver()
        {
        }

        public IJsonFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IJsonFormatter<T> formatter;

            [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("AOT", "IL3050", Justification = "The Nullable<enum> branch only runs when DynamicCodeSupport.IsSupported.")]
            [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("Trimming", "IL2055", Justification = "StaticNullableFormatter<T> is constructed over an enum (a value type), which always satisfies its struct/new() requirement.")]
            [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("Trimming", "IL2066", Justification = "Same as IL2055: the generic argument is an enum (a value type).")]
            [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("Trimming", "IL2091", Justification = "Same as IL2055: value types always have a parameterless constructor.")]
            static FormatterCache()
            {
                var ti = typeof(T).GetTypeInfo();

                if (ti.IsNullable())
                {
                    // build underlying type and use wrapped formatter.
                    ti = ti.GenericTypeArguments[0].GetTypeInfo();
                    if (!ti.IsEnum)
                    {
                        return;
                    }

                    // Nullable<enum> needs MakeGenericType over the enum, which native AOT cannot guarantee: decline there.
                    if (!Utf8Json.Internal.DynamicCodeSupport.IsSupported)
                    {
                        return;
                    }

                    var innerFormatter = Instance.GetFormatterDynamic(ti.AsType());
                    if (innerFormatter == null)
                    {
                        return;
                    }
                    formatter = (IJsonFormatter<T>)Activator.CreateInstance(typeof(StaticNullableFormatter<>).MakeGenericType(ti.AsType()), new object[] { innerFormatter });
                    return;
                }
                else if (typeof(T).IsEnum)
                {
                    formatter = (IJsonFormatter<T>)(object)new EnumFormatter<T>(true);
                }
            }
        }
    }

    internal sealed class EnumUnderlyingValueResolver : IJsonFormatterResolver
    {
        public static readonly IJsonFormatterResolver Instance = new EnumUnderlyingValueResolver();

        EnumUnderlyingValueResolver()
        {
        }

        public IJsonFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IJsonFormatter<T> formatter;

            [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("AOT", "IL3050", Justification = "The Nullable<enum> branch only runs when DynamicCodeSupport.IsSupported.")]
            [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("Trimming", "IL2055", Justification = "StaticNullableFormatter<T> is constructed over an enum (a value type), which always satisfies its struct/new() requirement.")]
            [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("Trimming", "IL2066", Justification = "Same as IL2055: the generic argument is an enum (a value type).")]
            [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("Trimming", "IL2091", Justification = "Same as IL2055: value types always have a parameterless constructor.")]
            static FormatterCache()
            {
                var ti = typeof(T).GetTypeInfo();

                if (ti.IsNullable())
                {
                    // build underlying type and use wrapped formatter.
                    ti = ti.GenericTypeArguments[0].GetTypeInfo();
                    if (!ti.IsEnum)
                    {
                        return;
                    }

                    // Nullable<enum> needs MakeGenericType over the enum, which native AOT cannot guarantee: decline there.
                    if (!Utf8Json.Internal.DynamicCodeSupport.IsSupported)
                    {
                        return;
                    }

                    var innerFormatter = Instance.GetFormatterDynamic(ti.AsType());
                    if (innerFormatter == null)
                    {
                        return;
                    }
                    formatter = (IJsonFormatter<T>)Activator.CreateInstance(typeof(StaticNullableFormatter<>).MakeGenericType(ti.AsType()), new object[] { innerFormatter });
                    return;
                }
                else if (typeof(T).IsEnum)
                {
                    formatter = (IJsonFormatter<T>)(object)new EnumFormatter<T>(false);
                }
            }
        }
    }
}