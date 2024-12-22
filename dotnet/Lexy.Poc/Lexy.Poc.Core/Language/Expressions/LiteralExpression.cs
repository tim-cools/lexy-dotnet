using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;

namespace Lexy.Poc.Core.Language.Expressions
{
    public class LiteralExpression : Expression
    {
        public ILiteralToken Literal { get; }

        private LiteralExpression(Line sourceLine, TokenList tokens, ILiteralToken literal) : base(sourceLine, tokens)
        {
            Literal = literal;
        }

        public static Expression Parse(IParserContext context, Line sourceLine, TokenList tokens)
        {
            if (!IsValid(tokens))
            {
                context.Logger.Fail("Invalid LiteralExpression.");
                return null;
            }

            var literalToken = tokens.LiteralToken(0);

            return new LiteralExpression(sourceLine, tokens, literalToken);
        }

        public static bool IsValid(TokenList tokens)
        {
            return tokens.Length == 1
                   && tokens.IsLiteralToken(0);
        }
    }
}