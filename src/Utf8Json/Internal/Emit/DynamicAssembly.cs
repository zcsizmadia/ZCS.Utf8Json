using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Utf8Json.Internal.Emit
{
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(Utf8Json.Internal.DynamicCodeSupport.RequiresUnreferencedCodeMessage)]
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(Utf8Json.Internal.DynamicCodeSupport.RequiresDynamicCodeMessage)]
    internal class DynamicAssembly
    {
        readonly string moduleName;

        // Created on first use, not in the constructor: resolvers hold a DynamicAssembly in static fields, and
        // creating the AssemblyBuilder eagerly made merely touching StandardResolver throw under native AOT.
        AssemblyBuilder assemblyBuilder;
        ModuleBuilder moduleBuilder;

        // don't expose ModuleBuilder
        // public ModuleBuilder ModuleBuilder { get { return moduleBuilder; } }

        readonly object gate = new object();

        // requires lock on mono environment. see: https://github.com/neuecc/MessagePack-CSharp/issues/161

        public TypeBuilder DefineType(string name, TypeAttributes attr)
        {
            lock (gate)
            {
                return GetModuleBuilder().DefineType(name, attr);
            }
        }

        public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent)
        {
            lock (gate)
            {
                return GetModuleBuilder().DefineType(name, attr, parent);
            }
        }

        public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent, Type[] interfaces)
        {
            lock (gate)
            {
                return GetModuleBuilder().DefineType(name, attr, parent, interfaces);
            }
        }

        public DynamicAssembly(string moduleName)
        {
            this.moduleName = moduleName;
        }

        // caller must hold gate
        ModuleBuilder GetModuleBuilder()
        {
            if (moduleBuilder != null) return moduleBuilder;

            if (!DynamicCodeSupport.IsSupported)
            {
                throw DynamicCodeSupport.CreateNotSupportedException("Building the dynamic assembly '" + moduleName + "'");
            }

#if NET45 || NET47
            assemblyBuilder = System.AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(moduleName), AssemblyBuilderAccess.RunAndSave);
            moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName, moduleName + ".dll");
#else
#if NETSTANDARD
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(moduleName), AssemblyBuilderAccess.Run);
#else
            assemblyBuilder = System.AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(moduleName), AssemblyBuilderAccess.Run);
#endif

            moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);
#endif
            return moduleBuilder;
        }

#if NET45 || NET47

        public AssemblyBuilder Save()
        {
            lock (gate)
            {
                GetModuleBuilder();
                assemblyBuilder.Save(moduleName + ".dll");
                return assemblyBuilder;
            }
        }

#endif
    }
}
