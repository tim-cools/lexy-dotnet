using System.Collections.Generic;
using System.Linq;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public class FunctionIncludes : IComponent
    {
        public IList<FunctionInclude> Definitions { get; } = new List<FunctionInclude>();

        public IComponent Parse(ParserContext parserContext)
        {
            var line = parserContext.CurrentLine;
            if (line.IsEmpty()) return this;

            var definition = FunctionInclude.Parse(line);
            Definitions.Add(definition);
            return this;
        }

        public bool Contains(string type)
        {
            return Definitions.Any(definition => definition.Name == type);
        }
    }
}