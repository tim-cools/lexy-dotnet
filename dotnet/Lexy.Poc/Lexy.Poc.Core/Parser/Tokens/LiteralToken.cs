namespace Lexy.Poc.Core.Parser
{
    public class LiteralToken : Token
    {
        public LiteralToken(char value) : base(value)
        {
        }

        public override ParseTokenResult Parse(char value, ParserContext parserContext)
        {
            if (char.IsWhiteSpace(value))
            {
                return new ParseTokenResult(TokenStatus.Finished, KeywordOrLiteral());
            }
            if (char.IsLetterOrDigit(value) || value == ':')
            {
                AppendValue(value);
                return new ParseTokenResult(TokenStatus.InProgress);
            }
            return new ParseTokenResult(TokenStatus.InvalidToken);
        }

        public override ParseTokenResult Finalize(ParserContext parserContext)
        {
            return new ParseTokenResult(TokenStatus.Finished, KeywordOrLiteral());
        }

        private Token KeywordOrLiteral()
        {
            var keyword = Value;
            if (Keywords.Contains(keyword))
            {
                return new KeywordToken(keyword);
            }

            return this;
        }
    }
}