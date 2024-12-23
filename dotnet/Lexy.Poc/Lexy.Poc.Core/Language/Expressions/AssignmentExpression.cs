using System.Collections.Generic;
using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;

namespace Lexy.Poc.Core.Language.Expressions
{
    public class AssignmentExpression : Expression
    {
        public string VariableName { get; }
        public Expression Assignment { get; }

        private AssignmentExpression(string variableName, Expression assignment, ExpressionSource source) : base(source)
        {
            VariableName = variableName;
            Assignment = assignment;
        }

        public static ParseExpressionResult Parse(ExpressionSource source)
        {
            var tokens = source.Tokens;
            if (!IsValid(tokens))
            {
                return ParseExpressionResult.Invalid<ParseExpressionResult>("Invalid expression.");
            }

            var variableName = tokens.TokenValue(0);
            var assignment = ExpressionFactory.Parse(tokens.TokensFrom(2), source.Line);

            var expression = new AssignmentExpression(variableName, assignment, source);

            return ParseExpressionResult.Success(expression);
        }

        public static bool IsValid(TokenList tokens)
        {
            return tokens.Length >= 3
                   && tokens.IsTokenType<StringLiteralToken>(0)
                   && tokens.OperatorToken(1, OperatorType.Assignment);
        }

        protected override IEnumerable<INode> GetChildren()
        {
            yield return Assignment;
        }

        protected override void Validate(IValidationContext context)
        {

        }
    }
}