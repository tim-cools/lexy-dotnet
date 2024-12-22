namespace Lexy.Poc.Core.Parser.Tokens
{
    internal class WhitespaceToken : ParsableToken
    {
        public WhitespaceToken(char value) : base(value)
        {
        }

        public override ParseTokenResult Parse(char value, IParserContext parserContext)
        {
            return !char.IsWhiteSpace(value)
                ? ParseTokenResult.Finished(false)
                : ParseTokenResult.InProgress();
        }

        public override ParseTokenResult Finalize(IParserContext parserContext)
        {
            return ParseTokenResult.Finished(true);
        }
    }
}