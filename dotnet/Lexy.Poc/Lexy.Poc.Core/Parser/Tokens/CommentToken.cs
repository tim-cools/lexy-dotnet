
namespace Lexy.Poc.Core.Parser
{
    public class CommentToken : Token
    {
        public CommentToken(char value) : base(value)
        {
        }

        public override ParseTokenResult Parse(char value, ParserContext parserContext)
        {
            AppendValue(value);
            return new ParseTokenResult(TokenStatus.InProgress);
        }

        public override ParseTokenResult Finalize(ParserContext parserContext)
        {
            return new ParseTokenResult(TokenStatus.Finished);
        }
    }
}