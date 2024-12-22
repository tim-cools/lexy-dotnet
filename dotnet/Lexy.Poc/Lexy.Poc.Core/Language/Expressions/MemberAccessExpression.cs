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

        public static Expression Parse(IParserContext context, Line sourceLine, TokenList tokens)
        {
            if (!IsValid(tokens))
            {
                context.Logger.Fail("Invalid MemberAccessExpression.");
                return null;
            }

            var variableName = tokens.TokenValue(0);

            return new MemberAccessExpression(sourceLine, tokens, variableName);
        }

        public static bool IsValid(TokenList tokens)
        {
            return tokens.Length == 1
                   && tokens.IsTokenType<MemberAccessLiteral>(0);
        }
    }
}