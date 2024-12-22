using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;

namespace Lexy.Poc.Core.Language.Expressions
{
    public class VariableExpression : Expression
    {
        public string VariableName { get; }

        private VariableExpression(Line sourceLine, TokenList tokens, string variableName) : base(sourceLine, tokens)
        {
            VariableName = variableName;
        }

        public static Expression Parse(IParserContext context, Line sourceLine, TokenList tokens)
        {
            if (!IsValid(tokens))
            {
                context.Logger.Fail("Invalid VariableExpression.");
                return null;
            }

            var assignmentVariableName = tokens.TokenValue(2);

            var variableName = tokens.TokenValue(0);

            return new VariableExpression(sourceLine, tokens, variableName);
        }

        public static bool IsValid(TokenList tokens)
        {
            return tokens.Length == 1
                   && tokens.IsTokenType<StringLiteralToken>(0);
        }
    }
}