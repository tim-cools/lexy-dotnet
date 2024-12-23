using System.Collections.Generic;
using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;

namespace Lexy.Poc.Core.Language.Expressions
{
    public class VariableExpression : Expression
    {
        public string VariableName { get; }

        private VariableExpression(string variableName, ExpressionSource source) : base(source)
        {
            VariableName = variableName;
        }

        public static ParseExpressionResult Parse(ExpressionSource source)
        {
            var tokens = source.Tokens;
            if (!IsValid(tokens))
            {
                return ParseExpressionResult.Invalid<VariableExpression>("Invalid expression");
            }

            var variableName = tokens.TokenValue(0);
            var expression = new VariableExpression(variableName, source);

            return ParseExpressionResult.Success(expression);
        }

        public static bool IsValid(TokenList tokens)
        {
            return tokens.Length == 1
                   && tokens.IsTokenType<StringLiteralToken>(0);
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