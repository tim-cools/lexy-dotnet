using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public class ScenarioExpectError : IComponent
    {
        public string Message { get; private set; }
        public bool HasValue { get => Message != null; }

        public IComponent Parse(ParserContext parserContext)
        {
            var line = parserContext.CurrentLine;
            Message = line.TokenValue(1);
            return this;
        }
    }
}