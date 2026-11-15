using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Utf8Json.Resolvers;
using Xunit;

namespace Utf8Json.Tests
{
    // Toggles a process-wide switch, so it must not run in parallel with other tests.
    [CollectionDefinition("NoDynamicCode", DisableParallelization = true)]
    public class NoDynamicCodeCollection { }

    /// <summary>
    /// Exercises the non-emit fallbacks used when RuntimeFeature.IsDynamicCodeSupported is false (native AOT),
    /// by simulating that state on the JIT. Every type here is used only by this class, because formatters
    /// and compiled delegates are cached per type for the lifetime of the process.
    /// </summary>
    [Collection("NoDynamicCode")]
    public class NoDynamicCodeFallbackTest
    {
        public enum FallbackColor { Red = 1, Blue = 2 }
        public enum FallbackLongColor : long { None = 0, Big = 1L << 40 }

        public class FallbackPoco
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class FallbackPoco2
        {
            public int X { get; set; }
        }

        public class UnformattablePoco
        {
            public int Z { get; set; }
        }

        public sealed class FallbackPocoFormatter : IJsonFormatter<FallbackPoco>
        {
            public void Serialize(ref JsonWriter writer, FallbackPoco value, IJsonFormatterResolver formatterResolver)
            {
                if (value == null) { writer.WriteNull(); return; }
                writer.WriteBeginObject();
                writer.WritePropertyName("Id"); writer.WriteInt32(value.Id);
                writer.WriteValueSeparator();
                writer.WritePropertyName("Name"); writer.WriteString(value.Name);
                writer.WriteEndObject();
            }

            public FallbackPoco Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
            {
                if (reader.ReadIsNull()) return null;
                var p = new FallbackPoco();
                var count = 0;
                reader.ReadIsBeginObjectWithVerify();
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    var name = reader.ReadPropertyName();
                    if (name == "Id") p.Id = reader.ReadInt32();
                    else if (name == "Name") p.Name = reader.ReadString();
                    else if (name == "Fail") throw new JsonParsingException("boom");
                    else reader.ReadNextBlock();
                }
                return p;
            }
        }

        sealed class Simulation : IDisposable
        {
            static readonly FieldInfo Field = typeof(JsonSerializer).Assembly
                .GetType("Utf8Json.Internal.DynamicCodeSupport", true)
                .GetField("simulateUnsupported", BindingFlags.Static | BindingFlags.NonPublic);

            public Simulation() { Field.SetValue(null, true); }
            public void Dispose() { Field.SetValue(null, false); }
        }

        static IJsonFormatterResolver CreateResolver()
        {
            return CompositeResolver.Create(new IJsonFormatter[] { new FallbackPocoFormatter() }, new[] { StandardResolver.Default });
        }

        [Fact]
        public void EnumUsesBoxedFallback()
        {
            using (new Simulation())
            {
                JsonSerializer.ToJsonString(FallbackColor.Blue, EnumResolver.UnderlyingValue).Is("2");
                JsonSerializer.Deserialize<FallbackColor>("2", EnumResolver.UnderlyingValue).Is(FallbackColor.Blue);
                JsonSerializer.ToJsonString(FallbackLongColor.Big, EnumResolver.UnderlyingValue).Is("1099511627776");
                JsonSerializer.Deserialize<FallbackLongColor>("1099511627776", EnumResolver.UnderlyingValue).Is(FallbackLongColor.Big);

                // by name does not need the delegates at all, but must still initialize without emit
                JsonSerializer.ToJsonString(FallbackColor.Red).Is("\"Red\"");
            }
        }

        [Fact]
        public void CompositeResolverCreateWithoutEmit()
        {
            using (new Simulation())
            {
                var resolver = CreateResolver();
                resolver.GetType().Name.Is("CachedCompositeResolver");

                JsonSerializer.ToJsonString(new FallbackPoco { Id = 1, Name = "x" }, resolver).Is("{\"Id\":1,\"Name\":\"x\"}");
                JsonSerializer.Deserialize<FallbackPoco>("{\"Id\":2,\"Name\":\"y\"}", resolver).Name.Is("y");
                JsonSerializer.ToJsonString(new[] { 1, 2 }, resolver).Is("[1,2]"); // falls through to StandardResolver
                Assert.Same(resolver.GetFormatter<FallbackPoco>(), resolver.GetFormatter<FallbackPoco>()); // cached
            }
        }

        [Fact]
        public void DynamicObjectResolverDeclinesWithClearMessage()
        {
            using (new Simulation())
            {
                var ex = Assert.Throws<FormatterNotRegisteredException>(() => JsonSerializer.ToJsonString(new UnformattablePoco()));
                Assert.Contains("UniversalCodeGenerator", ex.Message);
            }
        }

        [Fact]
        public void ObjectMemberUsesReflectionFallback()
        {
            using (new Simulation())
            {
                // a fresh fallback formatter, so nothing is served from a cache filled by the emit path
                var resolver = CreateResolver();
                var fallback = new Utf8Json.Formatters.DynamicObjectTypeFallbackFormatter(resolver);

                var writer = new JsonWriter();
                writer.WriteBeginArray();
                fallback.Serialize(ref writer, new FallbackPoco { Id = 3, Name = "z" }, resolver);
                writer.WriteValueSeparator();
                fallback.Serialize(ref writer, 5, resolver); // value type: unboxed by the reflective call
                writer.WriteEndArray();
                writer.ToString().Is("[{\"Id\":3,\"Name\":\"z\"},5]");

                // through StandardResolver's own object fallback, with a type no other test serializes
                JsonSerializer.ToJsonString(new object[] { FallbackColor.Blue }).Is("[\"Blue\"]");
            }
        }

        [Fact]
        public async Task NonGenericUsesReflectionFallback()
        {
            using (new Simulation())
            {
                var resolver = CreateResolver();
                var t = typeof(FallbackPoco);
                var v = new FallbackPoco { Id = 5, Name = "n" };
                var json = "{\"Id\":5,\"Name\":\"n\"}";

                JsonSerializer.NonGeneric.ToJsonString(t, v, resolver).Is(json);
                System.Text.Encoding.UTF8.GetString(JsonSerializer.NonGeneric.Serialize(t, v, resolver)).Is(json);

                var seg = JsonSerializer.NonGeneric.SerializeUnsafe(t, v, resolver);
                System.Text.Encoding.UTF8.GetString(seg.Array, seg.Offset, seg.Count).Is(json);

                var ms = new MemoryStream();
                JsonSerializer.NonGeneric.Serialize(t, ms, v, resolver);
                System.Text.Encoding.UTF8.GetString(ms.ToArray()).Is(json);

                // by-ref writer: the writer's state must be copied back after the reflective call
                var writer = new JsonWriter();
                writer.WriteBeginArray();
                JsonSerializer.NonGeneric.Serialize(t, ref writer, v, resolver);
                writer.WriteEndArray();
                writer.ToString().Is("[" + json + "]");

                ((FallbackPoco)JsonSerializer.NonGeneric.Deserialize(t, json, resolver)).Id.Is(5);
                ((FallbackPoco)JsonSerializer.NonGeneric.Deserialize(t, System.Text.Encoding.UTF8.GetBytes("xx" + json), 2, resolver)).Id.Is(5);
                ((FallbackPoco)JsonSerializer.NonGeneric.Deserialize(t, new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)), resolver)).Id.Is(5);

                // by-ref reader: its offset must advance past the value
                var reader = new JsonReader(System.Text.Encoding.UTF8.GetBytes("[" + json + ",7]"));
                reader.ReadIsBeginArrayWithVerify();
                ((FallbackPoco)JsonSerializer.NonGeneric.Deserialize(t, ref reader, resolver)).Name.Is("n");
                reader.ReadIsValueSeparatorWithVerify();
                reader.ReadInt32().Is(7);

                var ms2 = new MemoryStream();
                await JsonSerializer.NonGeneric.SerializeAsync(t, ms2, v, resolver);
                System.Text.Encoding.UTF8.GetString(ms2.ToArray()).Is(json);
                ((FallbackPoco)await JsonSerializer.NonGeneric.DeserializeAsync(t, new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)), resolver)).Id.Is(5);
            }
        }

        [Fact]
        public void NonGenericFallbackKeepsOriginalExceptionType()
        {
            using (new Simulation())
            {
                var resolver = CreateResolver();
                // must be the formatter's own exception, not TargetInvocationException
                Assert.Throws<JsonParsingException>(() => JsonSerializer.NonGeneric.Deserialize(typeof(FallbackPoco), "{\"Fail\":1}", resolver));
            }
        }
    }
}
