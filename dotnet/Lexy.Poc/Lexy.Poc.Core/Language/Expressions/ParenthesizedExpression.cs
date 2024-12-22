using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;

namespace Lexy.Poc.Core.Language.Expressions
{
    public class ParenthesizedExpression : Expression
    {
        public Expression Expression { get; }

        protected ParenthesizedExpression(Expression expression, Line sourceLine, TokenList tokens) : base(sourceLine, tokens)
        {
            Expression = expression;
        }

        public static ParseExpressionResult Parse(Line sourceLine, TokenList tokens)
        {
            if (!IsValid(tokens))
            {
                return ParseExpressionResult.Invalid<ParenthesizedExpression>("Not valid.");
            }

            var matchingClosingParenthesis = FindMatchingClosingParenthesis(tokens);
            if (matchingClosingParenthesis == -1)
            {
                return ParseExpressionResult.Invalid<ParenthesizedExpression>("No closing parentheses found.");
            }

            var innerExpressionTokens = tokens.TokensRange(1, matchingClosingParenthesis - 1);
            var parseResult = ExpressionFactory.Parse(innerExpressionTokens, sourceLine);

            var innerExpression = parseResult;

            var expression = new ParenthesizedExpression(innerExpression, sourceLine, tokens);
            return ParseExpressionResult.Success(expression);
        }

        private static int FindMatchingClosingParenthesis(TokenList tokens)
        {
            var count = 0;
            for (var index = 0; index < tokens.Length; index++)
            {
                var token = tokens[index];
                if (!(token is OperatorToken operatorToken)) continue;

                if (operatorToken.Type == OperatorType.OpenParentheses)
                {
                    count++;
                }
                else if (operatorToken.Type == OperatorType.CloseParentheses)
                {
                    count--;
                    if (count == 0)
                    {
                        return index;
                    }
                }
            }

            return -1;
        }

        public static bool IsValid(TokenList tokens)
        {
            return tokens.OperatorToken(0, OperatorType.OpenParentheses);
        }
    }
}