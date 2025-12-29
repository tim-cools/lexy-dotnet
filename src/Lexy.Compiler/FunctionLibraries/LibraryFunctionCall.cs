using Lexy.Compiler.Language;
using Lexy.Compiler.Language.Functions;
using Lexy.Compiler.Language.VariableTypes;

namespace Lexy.Compiler.FunctionLibraries;

internal class LibraryFunctionCall : IInstanceFunctionCall
{
    public IdentifierPath FullTypeName { get; }
    public VariableType ReturnType { get; }

    public LibraryFunctionCall(IdentifierPath fullTypeName, VariableType returnType)
    {
        FullTypeName = fullTypeName;
        ReturnType = returnType;
    }
}