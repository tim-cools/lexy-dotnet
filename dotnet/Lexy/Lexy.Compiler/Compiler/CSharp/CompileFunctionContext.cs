using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lexy.Compiler.Compiler.CSharp.BuiltInFunctions;
using Lexy.Compiler.Language;
using Lexy.Compiler.Language.Expressions.Functions;

namespace Lexy.Compiler.Compiler.CSharp
{
    internal class CompileFunctionContext : ICompileFunctionContext
    {
        public Function Function { get; }
        public IEnumerable<BuiltInFunctionCall> BuiltInFunctionCalls { get; }

        public CompileFunctionContext(Function function, IEnumerable<BuiltInFunctionCall> builtInFunctionCalls)
        {
            Function = function ?? throw new ArgumentNullException(nameof(function));
            BuiltInFunctionCalls = builtInFunctionCalls ?? throw new ArgumentNullException(nameof(builtInFunctionCalls));
        }

        public BuiltInFunctionCall Get(BuiltInFunction builtInFunction)
        {
            return BuiltInFunctionCalls.FirstOrDefault(call => call.BuiltInFunction == builtInFunction);
        }

        public string ClassName()
        {
            var nameBuilder = new StringBuilder("Function");
            foreach (var @char in Function.Name.Value.Where(char.IsLetter))
            {
                nameBuilder.Append(@char);
            }
            return nameBuilder.ToString();
        }
    }
}