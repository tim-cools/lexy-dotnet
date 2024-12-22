using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;

namespace Lexy.Poc.Core.Language.Expressions
{
    public class AssignmentExpression : Expression
    {
        public string VariableName { get; }
        public Expression Assignment { get; }

        private AssignmentExpression(Line sourceLine, TokenList tokens, string variableName, Expression assignment) : base(sourceLine, tokens)
        {
            VariableName = variableName;
            Assignment = assignment;
        }

        public static Expression Parse(IParserContext context, Line sourceLine, TokenList tokens)
        {
            if (!IsValid(tokens))
            {
                context.Logger.Fail("Invalid AssignmentExpression.");
                return null;
            }

            var variableName = tokens.TokenValue(0);
            var assignment = ExpressionFactory.Parse(context, tokens.TokensFrom(2));

            return new AssignmentExpression(sourceLine, tokens, variableName, assignment);
        }

        public static bool IsValid(TokenList tokens)
        {
            return tokens.Length >= 3
                   && tokens.IsTokenType<StringLiteralToken>(0)
                   && tokens.OperatorToken(1, OperatorType.Assignment);
        }
    }
}