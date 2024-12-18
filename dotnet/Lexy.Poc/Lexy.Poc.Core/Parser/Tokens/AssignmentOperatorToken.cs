namespace Lexy.Poc.Core.Parser
{
    public class AssignmentOperatorToken : ParsableToken
    {
        public AssignmentOperatorToken() : base(TokenValues.Assignment)
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