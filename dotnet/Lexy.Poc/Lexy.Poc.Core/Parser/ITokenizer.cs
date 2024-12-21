using Lexy.Poc.Core.Parser.Tokens;

namespace Lexy.Poc.Core.Parser
{
    public interface ITokenizer
    {
        Token[] Tokenize(Line line, ParserContext parserContext, out bool errors);
    }
}