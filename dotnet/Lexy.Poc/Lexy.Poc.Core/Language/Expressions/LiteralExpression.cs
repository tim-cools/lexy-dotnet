using System.Collections.Generic;
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

        public static ParseExpressionResult Parse(Line sourceLine, TokenList tokens)
        {
            if (!IsValid(tokens))
            {
                return ParseExpressionResult.Invalid<LiteralExpression>("Invalid expression.");
            }

            var literalToken = tokens.LiteralToken(0);

            var expression = new LiteralExpression(sourceLine, tokens, literalToken);
            return ParseExpressionResult.Success(expression);
        }

        public static bool IsValid(TokenList tokens)
        {
            return tokens.Length == 1
                   && tokens.IsLiteralToken(0);
        }

        protected override IEnumerable<INode> GetChildren()
        {
            yield break;
        }

        protected override void Validate(IParserContext context)
        {
        }
    }
}