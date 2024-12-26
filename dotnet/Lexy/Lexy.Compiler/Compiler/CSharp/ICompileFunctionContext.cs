using System.Collections.Generic;
using Lexy.Compiler.Compiler.CSharp.BuiltInFunctions;
using Lexy.Compiler.Language.Expressions.Functions;

namespace Lexy.Compiler.Compiler.CSharp
{
    internal interface ICompileFunctionContext
    {
        IEnumerable<BuiltInFunctionCall> BuiltInFunctionCalls { get; }

        BuiltInFunctionCall Get(BuiltInFunction expressionBuiltInFunction);
    }
}