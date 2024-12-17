using System;
using System.Linq;
using Lexy.Poc.Core.Language;
using Lexy.Poc.Core.Parser;
using Shouldly;

namespace Lexy.Poc
{
    public static class ParserExtensions
    {
        public static Function ParseFunction(this LexyParser parser, string code)
        {
            var codeLines = code.Split(Environment.NewLine);

            var typeSystem = parser.Parse(codeLines);

            if (typeSystem.Count != 1)
            {
                throw new InvalidOperationException("Only 1 token expected. Actual: " + typeSystem.Count);
            }

            var first = typeSystem.First();
            var function = first as Function;
            if (function == null)
            {
                throw new InvalidOperationException("Token not a function. Actual: " + first?.GetType());
            }
            function.FailedMessages.ShouldBeEmpty();

            return function;
        }

        public static Components ParseFunctionCode(this LexyParser parser, string code)
        {
            var codeLines = code.Split(Environment.NewLine);
            var typeSystem = parser.Parse(codeLines);

            if (typeSystem.Count != 1)
            {
                throw new InvalidOperationException("Only 1 token expected. Actual: " + typeSystem.Count);
            }

            var first = typeSystem.First();
            var function = first as Function;
            if (function == null)
            {
                throw new InvalidOperationException("Token not a function. Actual: " + first?.GetType());
            }
            function.FailedMessages.ShouldBeEmpty();

            return typeSystem;
        }
    }
}