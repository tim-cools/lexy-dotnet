namespace Lexy.Poc.Core.Parser.Tokens
{
    internal class WhitespaceToken : ParsableToken
    {
        public WhitespaceToken(char value) : base(value)
        {
        }

        public override ParseTokenResult Parse(char value, ParserContext parserContext)
        {
            return !char.IsWhiteSpace(value)
                ? ParseTokenResult.Finished(false)
                : ParseTokenResult.InProgress();
        }

        public override ParseTokenResult Finalize(ParserContext parserContext)
        {
            return ParseTokenResult.Finished(true);
        }
    }
}