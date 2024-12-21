namespace Lexy.Poc.Core.Parser.Tokens
{
    public class TableSeparatorToken : ParsableToken
    {
        public TableSeparatorToken() : base(TokenValues.TableSeparator)
        {
        }

        public override ParseTokenResult Parse(char value, ParserContext context)
        {
            return ParseTokenResult.Finished(true);
        }

        public override ParseTokenResult Finalize(ParserContext parserContext)
        {
            return ParseTokenResult.Finished(true);
        }
    }
}