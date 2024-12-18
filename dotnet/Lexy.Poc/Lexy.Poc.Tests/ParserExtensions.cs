using System;
using Lexy.Poc.Core.Language;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc
{
    public static class ParserExtensions
    {
        public static Components ParseComponents(this LexyParser parser, string code)
        {
            var codeLines = code.Split(Environment.NewLine);
            var context = parser.Parse(codeLines, true);

            return context.Components;
        }

        public static Function ParseFunction(this LexyParser parser, string code) => parser.ParseComponent<Function>(code);
        public static Table ParseTable(this LexyParser parser, string code) => parser.ParseComponent<Table>(code);

        public static T ParseComponent<T>(this LexyParser parser, string code) where T : RootComponent
        {
            var components = parser.ParseComponents(code);

            if (components.Count != 1)
            {
                throw new InvalidOperationException("Only 1 component expected. Actual: " + components.Count);
            }

            var first = components.First();
            if (!(first is T component))
            {
                throw new InvalidOperationException($"Component not a {typeof(T).Name}. Actual: {first?.GetType()}");
            }

            return component;
        }
    }
}