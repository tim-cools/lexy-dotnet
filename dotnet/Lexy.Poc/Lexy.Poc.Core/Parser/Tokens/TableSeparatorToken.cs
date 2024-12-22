namespace Lexy.Poc.Core.Parser.Tokens
{
    public class TableSeparatorToken : ParsableToken
    {
        public TableSeparatorToken() : base(TokenValues.TableSeparator)
        {
        }

        public override ParseTokenResult Parse(char value, IParserContext context)
        {
            return ParseTokenResult.Finished(true);
        }

        public override ParseTokenResult Finalize(IParserContext parserContext)
        {
            return ParseTokenResult.Finished(true);
        }
    }
}