#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

namespace AotSmoke.Resolvers
{
    using System;
    using Utf8Json;

    public class GeneratedResolver : global::Utf8Json.IJsonFormatterResolver
    {
        public static readonly global::Utf8Json.IJsonFormatterResolver Instance = new GeneratedResolver();

        GeneratedResolver()
        {

        }

        public global::Utf8Json.IJsonFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly global::Utf8Json.IJsonFormatter<T> formatter;

            static FormatterCache()
            {
                var f = GeneratedResolverGetFormatterHelper.GetFormatter(typeof(T));
                if (f != null)
                {
                    formatter = (global::Utf8Json.IJsonFormatter<T>)f;
                }
            }
        }
    }

    internal static class GeneratedResolverGetFormatterHelper
    {
        static readonly global::System.Collections.Generic.Dictionary<Type, int> lookup;

        static GeneratedResolverGetFormatterHelper()
        {
            lookup = new global::System.Collections.Generic.Dictionary<Type, int>(7)
            {
                {typeof(global::AotSmoke.Models.Status?), 0 },
                {typeof(global::System.Collections.Generic.List<global::System.Guid>), 1 },
                {typeof(global::System.Collections.Generic.Dictionary<string, long>), 2 },
                {typeof(global::System.Collections.Generic.List<global::AotSmoke.Models.OrderLine>), 3 },
                {typeof(global::AotSmoke.Models.OrderLine), 4 },
                {typeof(global::AotSmoke.Models.Address), 5 },
                {typeof(global::AotSmoke.Models.Order), 6 },
            };
        }

        internal static object GetFormatter(Type t)
        {
            int key;
            if (!lookup.TryGetValue(t, out key)) return null;

            switch (key)
            {
                case 0: return new global::Utf8Json.Formatters.NullableFormatter<global::AotSmoke.Models.Status>();
                case 1: return new global::Utf8Json.Formatters.ListFormatter<global::System.Guid>();
                case 2: return new global::Utf8Json.Formatters.DictionaryFormatter<string, long>();
                case 3: return new global::Utf8Json.Formatters.ListFormatter<global::AotSmoke.Models.OrderLine>();
                case 4: return new AotSmoke.Formatters.AotSmoke.Models.OrderLineFormatter();
                case 5: return new AotSmoke.Formatters.AotSmoke.Models.AddressFormatter();
                case 6: return new AotSmoke.Formatters.AotSmoke.Models.OrderFormatter();
                default: return null;
            }
        }
    }
}

#pragma warning disable 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 219
#pragma warning disable 168

namespace AotSmoke.Formatters.AotSmoke.Models
{
    using System;
    using Utf8Json;


    public sealed class OrderLineFormatter : global::Utf8Json.IJsonFormatter<global::AotSmoke.Models.OrderLine>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public OrderLineFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Sku"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Quantity"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Price"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("Sku"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Quantity"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Price"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::AotSmoke.Models.OrderLine value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.Sku);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteInt32(value.Quantity);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteDouble(value.Price);
            
            writer.WriteEndObject();
        }

        public global::AotSmoke.Models.OrderLine Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __Sku__ = default(string);
            var __Sku__b__ = false;
            var __Quantity__ = default(int);
            var __Quantity__b__ = false;
            var __Price__ = default(double);
            var __Price__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __Sku__ = reader.ReadString();
                        __Sku__b__ = true;
                        break;
                    case 1:
                        __Quantity__ = reader.ReadInt32();
                        __Quantity__b__ = true;
                        break;
                    case 2:
                        __Price__ = reader.ReadDouble();
                        __Price__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::AotSmoke.Models.OrderLine();
            if(__Sku__b__) ____result.Sku = __Sku__;
            if(__Quantity__b__) ____result.Quantity = __Quantity__;
            if(__Price__b__) ____result.Price = __Price__;

            return ____result;
        }
    }


    public sealed class AddressFormatter : global::Utf8Json.IJsonFormatter<global::AotSmoke.Models.Address>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public AddressFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("City"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Zip"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("City"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Zip"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::AotSmoke.Models.Address value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.City);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.Zip);
            
            writer.WriteEndObject();
        }

        public global::AotSmoke.Models.Address Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            

            var __City__ = default(string);
            var __City__b__ = false;
            var __Zip__ = default(string);
            var __Zip__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __City__ = reader.ReadString();
                        __City__b__ = true;
                        break;
                    case 1:
                        __Zip__ = reader.ReadString();
                        __Zip__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::AotSmoke.Models.Address(__City__, __Zip__);

            return ____result;
        }
    }


    public sealed class OrderFormatter : global::Utf8Json.IJsonFormatter<global::AotSmoke.Models.Order>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public OrderFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Id"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("customer"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Status"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("PreviousStatus"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Total"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Created"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Priority"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Tags"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Counters"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Numbers"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Lines"), 10},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("ShipTo"), 11},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("Id"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("customer"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Status"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("PreviousStatus"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Total"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Created"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Priority"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Tags"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Counters"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Numbers"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Lines"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("ShipTo"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::AotSmoke.Models.Order value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt32(value.Id);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.Customer);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::AotSmoke.Models.Status>().Serialize(ref writer, value.Status, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<global::AotSmoke.Models.Status?>().Serialize(ref writer, value.PreviousStatus, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<decimal>().Serialize(ref writer, value.Total, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.Created, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[6]);
            formatterResolver.GetFormatterWithVerify<int?>().Serialize(ref writer, value.Priority, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[7]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::System.Guid>>().Serialize(ref writer, value.Tags, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[8]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.Dictionary<string, long>>().Serialize(ref writer, value.Counters, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[9]);
            formatterResolver.GetFormatterWithVerify<int[]>().Serialize(ref writer, value.Numbers, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[10]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::AotSmoke.Models.OrderLine>>().Serialize(ref writer, value.Lines, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[11]);
            formatterResolver.GetFormatterWithVerify<global::AotSmoke.Models.Address>().Serialize(ref writer, value.ShipTo, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::AotSmoke.Models.Order Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __Id__ = default(int);
            var __Id__b__ = false;
            var __Customer__ = default(string);
            var __Customer__b__ = false;
            var __Status__ = default(global::AotSmoke.Models.Status);
            var __Status__b__ = false;
            var __PreviousStatus__ = default(global::AotSmoke.Models.Status?);
            var __PreviousStatus__b__ = false;
            var __Total__ = default(decimal);
            var __Total__b__ = false;
            var __Created__ = default(global::System.DateTime);
            var __Created__b__ = false;
            var __Priority__ = default(int?);
            var __Priority__b__ = false;
            var __Tags__ = default(global::System.Collections.Generic.List<global::System.Guid>);
            var __Tags__b__ = false;
            var __Counters__ = default(global::System.Collections.Generic.Dictionary<string, long>);
            var __Counters__b__ = false;
            var __Numbers__ = default(int[]);
            var __Numbers__b__ = false;
            var __Lines__ = default(global::System.Collections.Generic.List<global::AotSmoke.Models.OrderLine>);
            var __Lines__b__ = false;
            var __ShipTo__ = default(global::AotSmoke.Models.Address);
            var __ShipTo__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __Id__ = reader.ReadInt32();
                        __Id__b__ = true;
                        break;
                    case 1:
                        __Customer__ = reader.ReadString();
                        __Customer__b__ = true;
                        break;
                    case 2:
                        __Status__ = formatterResolver.GetFormatterWithVerify<global::AotSmoke.Models.Status>().Deserialize(ref reader, formatterResolver);
                        __Status__b__ = true;
                        break;
                    case 3:
                        __PreviousStatus__ = formatterResolver.GetFormatterWithVerify<global::AotSmoke.Models.Status?>().Deserialize(ref reader, formatterResolver);
                        __PreviousStatus__b__ = true;
                        break;
                    case 4:
                        __Total__ = formatterResolver.GetFormatterWithVerify<decimal>().Deserialize(ref reader, formatterResolver);
                        __Total__b__ = true;
                        break;
                    case 5:
                        __Created__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __Created__b__ = true;
                        break;
                    case 6:
                        __Priority__ = formatterResolver.GetFormatterWithVerify<int?>().Deserialize(ref reader, formatterResolver);
                        __Priority__b__ = true;
                        break;
                    case 7:
                        __Tags__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::System.Guid>>().Deserialize(ref reader, formatterResolver);
                        __Tags__b__ = true;
                        break;
                    case 8:
                        __Counters__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.Dictionary<string, long>>().Deserialize(ref reader, formatterResolver);
                        __Counters__b__ = true;
                        break;
                    case 9:
                        __Numbers__ = formatterResolver.GetFormatterWithVerify<int[]>().Deserialize(ref reader, formatterResolver);
                        __Numbers__b__ = true;
                        break;
                    case 10:
                        __Lines__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::AotSmoke.Models.OrderLine>>().Deserialize(ref reader, formatterResolver);
                        __Lines__b__ = true;
                        break;
                    case 11:
                        __ShipTo__ = formatterResolver.GetFormatterWithVerify<global::AotSmoke.Models.Address>().Deserialize(ref reader, formatterResolver);
                        __ShipTo__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::AotSmoke.Models.Order();
            if(__Id__b__) ____result.Id = __Id__;
            if(__Customer__b__) ____result.Customer = __Customer__;
            if(__Status__b__) ____result.Status = __Status__;
            if(__PreviousStatus__b__) ____result.PreviousStatus = __PreviousStatus__;
            if(__Total__b__) ____result.Total = __Total__;
            if(__Created__b__) ____result.Created = __Created__;
            if(__Priority__b__) ____result.Priority = __Priority__;
            if(__Tags__b__) ____result.Tags = __Tags__;
            if(__Counters__b__) ____result.Counters = __Counters__;
            if(__Numbers__b__) ____result.Numbers = __Numbers__;
            if(__Lines__b__) ____result.Lines = __Lines__;
            if(__ShipTo__b__) ____result.ShipTo = __ShipTo__;

            return ____result;
        }
    }

}

#pragma warning disable 168
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
