
namespace Lexy.Poc.Core.Parser.Tokens
{
    public class CommentToken : ParsableToken
    {
        public CommentToken(char value) : base(value)
        {
        }

        public override ParseTokenResult Parse(char value, ParserContext parserContext)
        {
            AppendValue(value);
            return ParseTokenResult.InProgress();
        }

        public override ParseTokenResult Finalize(ParserContext parserContext)
        {
            return ParseTokenResult.Finished(true);
        }
    }
}