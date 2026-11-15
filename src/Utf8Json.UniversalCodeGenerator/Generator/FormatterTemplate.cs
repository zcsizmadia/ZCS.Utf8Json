using System.Linq;
using Utf8Json.UniversalCodeGenerator;
using static Utf8Json.CodeGenerator.Generator.CodeWriter;

namespace Utf8Json.CodeGenerator.Generator
{
    /// <summary>
    /// Generates one IJsonFormatter&lt;T&gt; per collected object type, for all types of one namespace.
    /// </summary>
    public class FormatterTemplate
    {
        public string Namespace;
        public ObjectSerializationInfo[] objectSerializationInfos;

        public string TransformText()
        {
            var w = new CodeWriter();

            w.Line("#pragma warning disable 618");
            w.Line("#pragma warning disable 612");
            w.Line("#pragma warning disable 414");
            w.Line("#pragma warning disable 219");
            w.Line("#pragma warning disable 168");
            w.Line();
            w.Line("namespace " + Namespace);
            w.Line("{");
            w.Line("    using System;");
            w.Line("    using Utf8Json;");
            w.Line();

            foreach (var objInfo in objectSerializationInfos)
            {
                WriteFormatter(w, objInfo);
            }

            w.Line("}");
            w.Line();
            w.Line("#pragma warning disable 168");
            w.Line("#pragma warning restore 219");
            w.Line("#pragma warning restore 414");
            w.Line("#pragma warning restore 618");
            w.Text("#pragma warning restore 612");

            return w.ToString();
        }

        static void WriteFormatter(CodeWriter w, ObjectSerializationInfo objInfo)
        {
            w.Line();
            w.Line("    public sealed class " + objInfo.Name + "Formatter : global::Utf8Json.IJsonFormatter<" + objInfo.FullName + ">");
            w.Line("    {");
            w.Line("        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;");
            w.Line("        readonly byte[][] ____stringByteKeys;");
            w.Line();

            // constructor: key lookup for deserialize, pre-encoded property names for serialize
            w.Line("        public " + objInfo.Name + "Formatter()");
            w.Line("        {");
            w.Line("            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()");
            w.Line("            {");
            var index = 0;
            foreach (var x in objInfo.Members)
            {
                w.Line("                { JsonWriter.GetEncodedPropertyNameWithoutQuotation(\"" + x.Name + "\"), " + N(index++) + "},");
            }
            w.Line("            };");
            w.Line();
            w.Line("            this.____stringByteKeys = new byte[][]");
            w.Line("            {");
            index = 0;
            foreach (var x in objInfo.Members.Where(x => x.IsReadable))
            {
                var method = (index++ == 0) ? "GetEncodedPropertyNameWithBeginObject" : "GetEncodedPropertyNameWithPrefixValueSeparator";
                w.Line("                JsonWriter." + method + "(\"" + x.Name + "\"),");
            }
            w.Line("                ");
            w.Line("            };");
            w.Line("        }");
            w.Line();

            // Serialize
            w.Line("        public void Serialize(ref JsonWriter writer, " + objInfo.FullName + " value, global::Utf8Json.IJsonFormatterResolver formatterResolver)");
            w.Line("        {");
            if (objInfo.IsClass)
            {
                w.Line("            if (value == null)");
                w.Line("            {");
                w.Line("                writer.WriteNull();");
                w.Line("                return;");
                w.Line("            }");
            }
            w.Line("            ");
            w.Line();
            index = 0;
            foreach (var x in objInfo.Members.Where(x => x.IsReadable))
            {
                w.Line("            writer.WriteRaw(this.____stringByteKeys[" + N(index++) + "]);");
                w.Line("            " + x.GetSerializeMethodString() + ";");
            }
            w.Line("            ");
            w.Line("            writer.WriteEndObject();");
            w.Line("        }");
            w.Line();

            // Deserialize
            w.Line("        public " + objInfo.FullName + " Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)");
            w.Line("        {");
            w.Line("            if (reader.ReadIsNull())");
            w.Line("            {");
            if (objInfo.IsClass)
            {
                w.Line("                return null;");
            }
            else
            {
                w.Line("                throw new InvalidOperationException(\"typecode is null, struct not supported\");");
            }
            w.Line("            }");
            w.Line("            ");

            if (!objInfo.HasConstructor)
            {
                w.Line("            ");
                w.Line("\t        throw new InvalidOperationException(\"generated serializer for IInterface does not support deserialize.\");");
            }
            else
            {
                WriteDeserializeBody(w, objInfo);
            }

            w.Line("        }");
            w.Line("    }");
            w.Line();
        }

        static void WriteDeserializeBody(CodeWriter w, ObjectSerializationInfo objInfo)
        {
            w.Line();
            foreach (var x in objInfo.Members)
            {
                w.Line("            var __" + x.MemberName + "__ = default(" + x.Type + ");");
                w.Line("            var __" + x.MemberName + "__b__ = false;");
            }
            w.Line();
            w.Line("            var ____count = 0;");
            w.Line("            reader.ReadIsBeginObjectWithVerify();");
            w.Line("            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))");
            w.Line("            {");
            w.Line("                var stringKey = reader.ReadPropertyNameSegmentRaw();");
            w.Line("                int key;");
            w.Line("                if (!____keyMapping.TryGetValueSafe(stringKey, out key))");
            w.Line("                {");
            w.Line("                    reader.ReadNextBlock();");
            w.Line("                    goto NEXT_LOOP;");
            w.Line("                }");
            w.Line();
            w.Line("                switch (key)");
            w.Line("                {");
            var index = 0;
            foreach (var x in objInfo.Members)
            {
                w.Line("                    case " + N(index++) + ":");
                w.Line("                        __" + x.MemberName + "__ = " + x.GetDeserializeMethodString() + ";");
                w.Line("                        __" + x.MemberName + "__b__ = true;");
                w.Line("                        break;");
            }
            w.Line("                    default:");
            w.Line("                        reader.ReadNextBlock();");
            w.Line("                        break;");
            w.Line("                }");
            w.Line();
            w.Line("                NEXT_LOOP:");
            w.Line("                continue;");
            w.Line("            }");
            w.Line();
            w.Line("            var ____result = new " + objInfo.GetConstructorString() + ";");
            foreach (var x in objInfo.Members.Where(x => x.IsWritable))
            {
                w.Line("            if(__" + x.MemberName + "__b__) ____result." + x.MemberName + " = __" + x.MemberName + "__;");
            }
            w.Line();
            w.Line("            return ____result;");
        }
    }
}
