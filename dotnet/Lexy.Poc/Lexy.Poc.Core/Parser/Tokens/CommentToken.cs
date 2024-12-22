
namespace Lexy.Poc.Core.Parser.Tokens
{
    public class CommentToken : ParsableToken
    {
        public CommentToken(char value) : base(value)
        {
        }

        public override ParseTokenResult Parse(char value, IParserContext parserContext)
        {
            AppendValue(value);
            return ParseTokenResult.InProgress();
        }

        public override ParseTokenResult Finalize(IParserContext parserContext)
        {
            return ParseTokenResult.Finished(true);
        }
    }
}