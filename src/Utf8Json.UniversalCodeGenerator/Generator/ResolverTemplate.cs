using Utf8Json.UniversalCodeGenerator;
using static Utf8Json.CodeGenerator.Generator.CodeWriter;

namespace Utf8Json.CodeGenerator.Generator
{
    /// <summary>
    /// Generates the resolver that maps each collected type to its formatter.
    /// </summary>
    public class ResolverTemplate
    {
        public string Namespace;
        public string FormatterNamespace { get; set; }
        public string ResolverName = "GeneratedResolver";
        public IResolverRegisterInfo[] registerInfos;

        public string TransformText()
        {
            var w = new CodeWriter();

            w.Line("#pragma warning disable 618");
            w.Line("#pragma warning disable 612");
            w.Line("#pragma warning disable 414");
            w.Line("#pragma warning disable 168");
            w.Line();
            w.Line("namespace " + Namespace);
            w.Line("{");
            w.Line("    using System;");
            w.Line("    using Utf8Json;");
            w.Line();
            w.Line("    public class " + ResolverName + " : global::Utf8Json.IJsonFormatterResolver");
            w.Line("    {");
            w.Line("        public static readonly global::Utf8Json.IJsonFormatterResolver Instance = new " + ResolverName + "();");
            w.Line();
            w.Line("        " + ResolverName + "()");
            w.Line("        {");
            w.Line();
            w.Line("        }");
            w.Line();
            w.Line("        public global::Utf8Json.IJsonFormatter<T> GetFormatter<T>()");
            w.Line("        {");
            w.Line("            return FormatterCache<T>.formatter;");
            w.Line("        }");
            w.Line();
            w.Line("        static class FormatterCache<T>");
            w.Line("        {");
            w.Line("            public static readonly global::Utf8Json.IJsonFormatter<T> formatter;");
            w.Line();
            w.Line("            static FormatterCache()");
            w.Line("            {");
            w.Line("                var f = " + ResolverName + "GetFormatterHelper.GetFormatter(typeof(T));");
            w.Line("                if (f != null)");
            w.Line("                {");
            w.Line("                    formatter = (global::Utf8Json.IJsonFormatter<T>)f;");
            w.Line("                }");
            w.Line("            }");
            w.Line("        }");
            w.Line("    }");
            w.Line();
            w.Line("    internal static class " + ResolverName + "GetFormatterHelper");
            w.Line("    {");
            w.Line("        static readonly global::System.Collections.Generic.Dictionary<Type, int> lookup;");
            w.Line();
            w.Line("        static " + ResolverName + "GetFormatterHelper()");
            w.Line("        {");
            w.Line("            lookup = new global::System.Collections.Generic.Dictionary<Type, int>(" + N(registerInfos.Length) + ")");
            w.Line("            {");
            for (var i = 0; i < registerInfos.Length; i++)
            {
                w.Line("                {typeof(" + registerInfos[i].FullName + "), " + N(i) + " },");
            }
            w.Line("            };");
            w.Line("        }");
            w.Line();
            w.Line("        internal static object GetFormatter(Type t)");
            w.Line("        {");
            w.Line("            int key;");
            w.Line("            if (!lookup.TryGetValue(t, out key)) return null;");
            w.Line();
            w.Line("            switch (key)");
            w.Line("            {");
            for (var i = 0; i < registerInfos.Length; i++)
            {
                w.Line("                case " + N(i) + ": return new " + GetFormatterFullName(registerInfos[i]) + "();");
            }
            w.Line("                default: return null;");
            w.Line("            }");
            w.Line("        }");
            w.Line("    }");
            w.Line("}");
            w.Line();
            w.Line("#pragma warning disable 168");
            w.Line("#pragma warning restore 414");
            w.Line("#pragma warning restore 618");
            w.Text("#pragma warning restore 612");

            return w.ToString();
        }

        string GetFormatterFullName(IResolverRegisterInfo info)
        {
            if (info.FormatterName.StartsWith("global::")) return info.FormatterName;
            return (!string.IsNullOrEmpty(FormatterNamespace) ? FormatterNamespace + "." : FormatterNamespace) + info.FormatterName;
        }
    }
}
