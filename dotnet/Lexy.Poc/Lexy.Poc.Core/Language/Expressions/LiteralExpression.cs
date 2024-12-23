using System.Collections.Generic;
using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;

namespace Lexy.Poc.Core.Language.Expressions
{
    public class LiteralExpression : Expression
    {
        public ILiteralToken Literal { get; }

        private LiteralExpression(ILiteralToken literal, ExpressionSource source) : base(source)
        {
            Literal = literal;
        }

        public static ParseExpressionResult Parse(ExpressionSource source)
        {
            var tokens = source.Tokens;
            if (!IsValid(tokens))
            {
                return ParseExpressionResult.Invalid<LiteralExpression>("Invalid expression.");
            }

            var literalToken = tokens.LiteralToken(0);

            var expression = new LiteralExpression(literalToken, source);
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