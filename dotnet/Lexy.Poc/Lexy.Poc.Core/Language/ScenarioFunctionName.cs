using Lexy.Poc.Core.Parser;
using Microsoft.Extensions.Configuration;

namespace Lexy.Poc.Core.Language
{
    public class ScenarioFunctionName : IComponent
    {
        public string Value { get; private set; }

        public IComponent Parse(ParserContext parserContext)
        {
            var line = parserContext.CurrentLine;
            Value = line.TokenValue(1);
            return this;
        }

        public override string ToString() => Value;
    }
}