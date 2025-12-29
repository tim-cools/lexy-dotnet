using Lexy.Compiler.Language.VariableTypes;
using Shouldly;

namespace Lexy.Tests.Parser;

internal static class VariableTypeExtensions
{
    public static void ShouldBePrimitiveType(this VariableTypeDeclaration type, string name)
    {
        type.ShouldBeOfType<PrimitiveVariableTypeDeclaration>()
            .Type.ShouldBe(name);
    }
}