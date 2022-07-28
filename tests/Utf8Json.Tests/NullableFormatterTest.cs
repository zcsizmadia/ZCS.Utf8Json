using System;
using System.Collections.Generic;
using Xunit;

namespace Utf8Json.Tests
{
    public class NullableFormatterTest
    {
        public class MyContainer
        {
            public List<int> field1 = new List<int>();
            public List<int?> field2 = new List<int?>();
            public List<string> field3 = new List<string>();
            public List<string?> field4 = new List<string?>();
            public List<DateTime> field5 = new List<DateTime>();
            public List<DateTime?> field6 = new List<DateTime?>();
            public List<MyClass> field7 = new List<MyClass>();
            public List<MyClass?> field8 = new List<MyClass?>();
            public List<MyStruct> field9 = new List<MyStruct>();
            public List<MyStruct?> field10 = new List<MyStruct?>();
        }

        [JsonFormatter(typeof(MyClassConverter))]
        public class MyClass
        {
            public int Value;

            public MyClass(int value) { Value = value; }
        }

        public class MyClassConverter : IJsonFormatter<MyClass>
        {
            public void Serialize(ref JsonWriter writer, MyClass value, IJsonFormatterResolver jsonFormatterResolver)
            {
                if (value == null)
                    writer.WriteNull();
                else
                    writer.WriteInt32(value.Value);
            }

            public MyClass Deserialize(ref JsonReader reader, IJsonFormatterResolver jsonFormatterResolver)
            {
                return new MyClass(reader.ReadInt32());
            }
        }

        [JsonFormatter(typeof(MyStructConverter))]
        public struct MyStruct
        {
            public int Value;

            public MyStruct(int value) { Value = value; }
        }

        public class MyStructConverter : IJsonFormatter<MyStruct>
        {
            public void Serialize(ref JsonWriter writer, MyStruct value, IJsonFormatterResolver jsonFormatterResolver)
            {
                writer.WriteInt32(value.Value);
            }

            public MyStruct Deserialize(ref JsonReader reader, IJsonFormatterResolver jsonFormatterResolver)
            {
                return new MyStruct(reader.ReadInt32());
            }
        }

        [Fact]
        public void Foo()
        {
            MyContainer container = new MyContainer();
            container.field1.Add(1);
            container.field1.Add(2);
            container.field2.Add(10);
            container.field2.Add(null);
            container.field3.Add("100");
            container.field3.Add("200");
            container.field4.Add("1000");
            container.field4.Add(null);
            container.field5.Add(DateTime.UnixEpoch);
            container.field5.Add(DateTime.UnixEpoch);
            container.field6.Add(DateTime.UnixEpoch);
            container.field6.Add(null);
            container.field7.Add(new MyClass(1));
            container.field7.Add(new MyClass(2));
            container.field8.Add(new MyClass(10));
            container.field8.Add(null);
            container.field9.Add(new MyStruct(100));
            container.field9.Add(new MyStruct(200));
            container.field10.Add(new MyStruct(1000));
            container.field10.Add(null);

            JsonSerializer.NonGeneric.ToJsonString(container).Is("{\"field1\":[1,2],\"field2\":[10,null],\"field3\":[\"100\",\"200\"],\"field4\":[\"1000\",null],\"field5\":[\"1970-01-01T00:00:00Z\",\"1970-01-01T00:00:00Z\"],\"field6\":[\"1970-01-01T00:00:00Z\",null],\"field7\":[1,2],\"field8\":[10,null],\"field9\":[100,200],\"field10\":[1000,null]}");
        }
    }
}
