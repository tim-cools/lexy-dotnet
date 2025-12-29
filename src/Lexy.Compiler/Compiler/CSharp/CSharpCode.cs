using System;
using Lexy.Compiler.Compiler.CSharp.Components;
using Lexy.Compiler.Language;
using Lexy.Compiler.Language.Enums;
using Lexy.Compiler.Language.Functions;
using Lexy.Compiler.Language.Scenarios;
using Lexy.Compiler.Language.Types;
using Table = Lexy.Compiler.Language.Tables.Table;

namespace Lexy.Compiler.Compiler.CSharp;

internal static class CSharpCode
{
    public static Func<IComponentNode, GeneratedClass> GetGenerator(IComponentNode componentNode)
    {
        return componentNode switch
        {
            Function _ => Cast<Function>(FunctionClass.CreateCode),
            EnumDefinition _ => Cast<EnumDefinition>(EnumClass.CreateCode),
            Table _ => Cast<Table>(TableClass.CreateCode),
            TypeDefinition _ => Cast<TypeDefinition>(TypeClass.CreateCode),
            Scenario _ => null,
            _ => throw new InvalidOperationException("No writer defined: " + componentNode.GetType())
        };
    }

    private static Func<IComponentNode, GeneratedClass> Cast<T>(Func<T, GeneratedClass> function) where T : class
    {
        return node =>
        {
            if (node is not T specific)
            {
                throw new InvalidOperationException($"Node is not: '{typeof(T)}'");
            }
            return function(specific);
        };
    }
}