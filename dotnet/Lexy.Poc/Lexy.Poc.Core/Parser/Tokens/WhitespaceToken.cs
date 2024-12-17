namespace Lexy.Poc.Core.Parser
{
    internal class WhitespaceToken : Token
    {
        public WhitespaceToken(char value) : base(value)
        {
        }

        public override ParseTokenResult Parse(char value, ParserContext parserContext)
        {
            var status = !char.IsWhiteSpace(value)
                ? TokenStatus.Finished
                : TokenStatus.InProgress;

            return new ParseTokenResult(status);
        }

        public override ParseTokenResult Finalize(ParserContext parserContext)
        {
            return new ParseTokenResult(TokenStatus.Finished);
        }
    }
}