namespace Lexy.Poc.Core.Parser
{
    internal interface ITokenizer
    {
        Token[] Tokenize(Line line, ParserContext parserContext);
    }
}