using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;

namespace Lexy.Poc.Core.Language.Expressions
{
    public class MemberAccessExpression : Expression
    {
        public string Value { get; }

        private MemberAccessExpression(Line sourceLine, TokenList tokens, string value) : base(sourceLine, tokens)
        {
            Value = value;
        }

        public static ParseExpressionResult Parse(Line sourceLine, TokenList tokens)
        {
            if (!IsValid(tokens))
            {
                return ParseExpressionResult.Invalid<MemberAccessExpression>("Invalid expression");
            }

            var variableName = tokens.TokenValue(0);

            var accessExpression = new MemberAccessExpression(sourceLine, tokens, variableName);
            return ParseExpressionResult.Success(accessExpression);
        }

        public static bool IsValid(TokenList tokens)
        {
            return tokens.Length == 1
                   && tokens.IsTokenType<MemberAccessLiteral>(0);
        }
    }
}