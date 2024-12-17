namespace Lexy.Poc.Core.Parser
{
    internal class NumericToken : Token
    {
        public NumericToken(char value) : base(value)
        {
        }

        public override ParseTokenResult Parse(char value, ParserContext parserContext)
        {
            if (char.IsDigit(value) || value == '.')
            {
                AppendValue(value);
                return new ParseTokenResult(TokenStatus.InProgress);
            }

            return new ParseTokenResult(TokenStatus.Finished);
        }

        public override ParseTokenResult Finalize(ParserContext parserContext)
        {
            return new ParseTokenResult(TokenStatus.Finished);
        }
    }
}