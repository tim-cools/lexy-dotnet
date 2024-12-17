using System;

namespace Lexy.Poc.Core.Parser
{
    public class KeywordToken : Token
    {
        public KeywordToken(string keyword) : base(keyword)
        {
        }

        public override ParseTokenResult Parse(char value, ParserContext parserContext)
        {
            return new ParseTokenResult(TokenStatus.Finished);
        }

        public override ParseTokenResult Finalize(ParserContext parserContext)
        {
            return new ParseTokenResult(TokenStatus.Finished);
        }
    }
}