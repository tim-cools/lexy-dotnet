using System;
using Lexy.Poc.Core.Compiler;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Compiler
{
    public static class LexyScript
    {
        public static ExecutableFunction Create(string code)
        {
            if (code == null) throw new ArgumentNullException(nameof(code));

            var parser = new LexyParser();
            var components = parser.ParseComponents(code);
            var function = components.GetSingleFunction();

            var compiler = new LexyCompiler();
            var environment = compiler.Compile(components, function);
            return environment.GetFunction(function);
        }
    }
}