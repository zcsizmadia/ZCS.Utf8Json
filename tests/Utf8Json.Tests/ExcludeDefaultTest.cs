using System;
using System.Reflection;
using Utf8Json.Resolvers;
using Xunit;

namespace Utf8Json.Tests
{
    public class ExcludeDefaultTest
    {
        [Fact]
        public void TestFilledValues()
        {
            var person = new SimplePerson()
            {
                Age = 18,
                Name = "Bob",
                Number = 3147483647,
                IsHuman = true,
                Probability = 0.7891m,
                Size = 178,
                Weight = 72.5f,
                Color = Color.Blue
            };

            var answer = "{\"Age\":18,\"Name\":\"Bob\",\"Number\":3147483647,\"IsHuman\":true,\"Probability\":0.7891,\"Size\":178,\"Weight\":72.5,\"Color\":\"Blue\"}";
            JsonSerializer.ToJsonString(person, StandardResolver.Default).Is(answer);
            JsonSerializer.ToJsonString(person, StandardResolver.ExcludeNull).Is(answer);
            JsonSerializer.ToJsonString(person, StandardResolver.ExcludeNullExcludeDefault).Is(answer);
            JsonSerializer.ToJsonString(person, StandardResolver.AllowPrivate).Is(answer);
            JsonSerializer.ToJsonString(person, StandardResolver.AllowPrivateExcludeNullExcludeDefault).Is(answer);
        }

        [Fact]
        public void TestDefaultValues()
        {
            var person = new SimplePerson();

            var answer1 = "{\"Age\":0,\"Name\":null,\"Number\":0,\"IsHuman\":false,\"Probability\":0,\"Size\":0,\"Weight\":0,\"Color\":\"None\"}";
            var answer2 = "{\"Age\":0,\"Number\":0,\"IsHuman\":false,\"Probability\":0,\"Size\":0,\"Weight\":0,\"Color\":\"None\"}";
            var answerDefault = "{}";
            JsonSerializer.ToJsonString(person, StandardResolver.Default).Is(answer1);
            JsonSerializer.ToJsonString(person, StandardResolver.ExcludeNull).Is(answer2);
            JsonSerializer.ToJsonString(person, StandardResolver.ExcludeNullExcludeDefault).Is(answerDefault);
            JsonSerializer.ToJsonString(person, StandardResolver.AllowPrivate).Is(answer1);
            JsonSerializer.ToJsonString(person, StandardResolver.AllowPrivateExcludeNullExcludeDefault).Is(answerDefault);
        }

        [Fact]
        public void TestNullValues()
        {
            var person = new IncludeNullablePerson();

            var answer3 = "{\"Age\":null,\"Name\":null}";
            var answer4 = "{}";
            JsonSerializer.ToJsonString(person, StandardResolver.Default).Is(answer3);
            JsonSerializer.ToJsonString(person, StandardResolver.ExcludeNull).Is(answer4);
            JsonSerializer.ToJsonString(person, StandardResolver.ExcludeNullExcludeDefault).Is(answer4);
            JsonSerializer.ToJsonString(person, StandardResolver.AllowPrivate).Is(answer3);
            JsonSerializer.ToJsonString(person, StandardResolver.AllowPrivateExcludeNull).Is(answer4);
            JsonSerializer.ToJsonString(person, StandardResolver.AllowPrivateExcludeNullExcludeDefault).Is(answer4);
        }

        [Fact]
        public void FloatIsKeptWhenNotDefault()
        {
            // regression: every float member used to be dropped, whatever its value
            var v = new WithFloat { F = 1.5f, I = 7 };
            JsonSerializer.ToJsonString(v, StandardResolver.ExcludeNullExcludeDefault).Is("{\"F\":1.5,\"I\":7}");
            JsonSerializer.ToJsonString(v, StandardResolver.AllowPrivateExcludeNullExcludeDefault).Is("{\"F\":1.5,\"I\":7}");
            JsonSerializer.ToJsonString(new WithFloat(), StandardResolver.ExcludeNullExcludeDefault).Is("{}");
        }

        [Fact]
        public void AllPrimitiveWidths()
        {
            var filled = new AllPrimitives
            {
                Bool = true,
                Byte = 1,
                SByte = -1,
                Short = -2,
                UShort = 2,
                Int = -3,
                UInt = 3,
                Long = 4294967296L, // low 32 bits are zero
                ULong = 4294967296UL,
                Char = 'a',
                Float = 0.5f,
                Double = 0.25,
                Decimal = 1.5m,
                LongEnum = LongColor.Blue,
                ByteEnum = ByteColor.Blue,
            };

            var expected = "{\"Bool\":true,\"Byte\":1,\"SByte\":-1,\"Short\":-2,\"UShort\":2,\"Int\":-3,\"UInt\":3,\"Long\":4294967296,\"ULong\":4294967296,\"Char\":\"a\",\"Float\":0.5,\"Double\":0.25,\"Decimal\":1.5,\"LongEnum\":\"Blue\",\"ByteEnum\":\"Blue\"}";
            JsonSerializer.ToJsonString(filled, StandardResolver.ExcludeNullExcludeDefault).Is(expected);
            JsonSerializer.ToJsonString(filled, StandardResolver.AllowPrivateExcludeNullExcludeDefault).Is(expected);

            JsonSerializer.ToJsonString(new AllPrimitives(), StandardResolver.ExcludeNullExcludeDefault).Is("{}");
            JsonSerializer.ToJsonString(new AllPrimitives(), StandardResolver.AllowPrivateExcludeNullExcludeDefault).Is("{}");
        }

        [Fact]
        public void NaNIsKept()
        {
            JsonSerializer.ToJsonString(new WithFloat { F = float.NaN }, StandardResolver.ExcludeNullExcludeDefault).Is("{\"F\":NaN}");
        }

        [Fact]
        public void NullableHoldingZeroIsWritten()
        {
            // a nullable's default is null, so 0 is a real value and is kept
            var v = new IncludeNullablePerson { Age = 0 };
            JsonSerializer.ToJsonString(v, StandardResolver.ExcludeNullExcludeDefault).Is("{\"Age\":0}");
            JsonSerializer.ToJsonString(v, StandardResolver.AllowPrivateExcludeNullExcludeDefault).Is("{\"Age\":0}");
        }

        [Fact]
        public void StructMemberIsNotAffected()
        {
            var v = new WithStruct();
            // the member itself is written; its own members follow the same rule
            JsonSerializer.ToJsonString(v, StandardResolver.ExcludeNullExcludeDefault).Is("{\"Inner\":{}}");
            JsonSerializer.ToJsonString(new WithStruct { Inner = new Point { X = 2 } }, StandardResolver.ExcludeNullExcludeDefault).Is("{\"Inner\":{\"X\":2}}");
        }

        [Fact]
        public void ExcludeDefaultWithoutExcludeNull()
        {
            // No public resolver has this combination yet; build it directly so the code paths stay consistent
            // for when one is added (this used to throw, or emit malformed JSON).
            var filled = new SimpleRecord { A = 1, N = "x" };
            var empty = new SimpleRecord();

            foreach (var formatter in new[] { BuildDynamicMethodFormatter<SimpleRecord>(), BuildAssemblyFormatter<SimpleRecord>() })
            {
                Serialize(formatter, filled).Is("{\"A\":1,\"N\":\"x\"}");
                Serialize(formatter, empty).Is("{\"N\":null}");
                Serialize(formatter, new SimpleRecord { N = "x" }).Is("{\"N\":\"x\"}");
            }
        }

        static string Serialize<T>(IJsonFormatter<T> formatter, T value)
        {
            var writer = new JsonWriter();
            formatter.Serialize(ref writer, value, StandardResolver.Default);
            return writer.ToString();
        }

        static readonly Type BuilderType = typeof(JsonSerializer).Assembly.GetType("Utf8Json.Resolvers.Internal.DynamicObjectTypeBuilder", true);
        static readonly Func<string, string> Original = s => s;

        static IJsonFormatter<T> BuildDynamicMethodFormatter<T>()
        {
            var m = BuilderType.GetMethod("BuildFormatterToDynamicMethod").MakeGenericMethod(typeof(T));
            return (IJsonFormatter<T>)m.Invoke(null, new object[] { StandardResolver.Default, Original, false, true, true });
        }

        static IJsonFormatter<T> BuildAssemblyFormatter<T>()
        {
            var asmType = typeof(JsonSerializer).Assembly.GetType("Utf8Json.Internal.Emit.DynamicAssembly", true);
            var asm = Activator.CreateInstance(asmType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { "ExcludeDefaultTest" + typeof(T).Name }, null);
            var m = BuilderType.GetMethod("BuildFormatterToAssembly").MakeGenericMethod(typeof(T));
            return (IJsonFormatter<T>)m.Invoke(null, new object[] { asm, StandardResolver.Default, Original, false, true });
        }

        public class SimplePerson
        {
            public int Age { get; set; }
            public string Name { get; set; }
            public long Number { get; set; }
            public bool IsHuman { get; set; }
            public decimal Probability { get; set; }
            public double Size { get; set; }
            public float Weight { get; set; }
            public Color Color { get; set; }
        }

        public class IncludeNullablePerson
        {
            public int? Age { get; set; }
            public string Name { get; set; }
        }

        public class WithFloat
        {
            public float F { get; set; }
            public int I { get; set; }
        }

        public class AllPrimitives
        {
            public bool Bool { get; set; }
            public byte Byte { get; set; }
            public sbyte SByte { get; set; }
            public short Short { get; set; }
            public ushort UShort { get; set; }
            public int Int { get; set; }
            public uint UInt { get; set; }
            public long Long { get; set; }
            public ulong ULong { get; set; }
            public char Char { get; set; }
            public float Float { get; set; }
            public double Double { get; set; }
            public decimal Decimal { get; set; }
            public LongColor LongEnum { get; set; }
            public ByteColor ByteEnum { get; set; }
        }

        public struct Point
        {
            public int X { get; set; }
        }

        public class WithStruct
        {
            public Point Inner { get; set; }
        }

        public class SimpleRecord
        {
            public int A { get; set; }
            public string N { get; set; }
        }

        public enum Color
        {
            None = 0,
            Blue = 1
        }

        public enum LongColor : long
        {
            None = 0,
            Blue = 1L << 40
        }

        public enum ByteColor : byte
        {
            None = 0,
            Blue = 1
        }
    }
}
