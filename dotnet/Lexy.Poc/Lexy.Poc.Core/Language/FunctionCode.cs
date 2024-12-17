using System.Collections.Generic;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public class FunctionCode : IComponent
    {
        public IList<string> Lines { get; } = new List<string>();

        public IComponent Parse(ParserContext parserContext)
        {
            var line = parserContext.CurrentLine;
            Lines.Add(line.TrimmedContent);
            return this;
        }
    }
}