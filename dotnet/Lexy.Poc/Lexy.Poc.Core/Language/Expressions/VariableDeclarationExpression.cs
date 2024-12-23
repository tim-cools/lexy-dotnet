using System;
using System.Collections.Generic;
using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;

namespace Lexy.Poc.Core.Language.Expressions
{
    public class VariableDeclarationExpression : Expression
    {
        public VariableType VariableType { get; }
        public string VariableName { get; }
        public Expression Assignment { get; }

        private VariableDeclarationExpression(VariableType variableType, string variableName, Expression assignment,
            ExpressionSource source, SourceReference reference) : base(source, reference)
        {
            VariableType = variableType ?? throw new ArgumentNullException(nameof(variableType));
            VariableName = variableName ?? throw new ArgumentNullException(nameof(variableName));
            Assignment = assignment;
        }

        public static ParseExpressionResult Parse(ExpressionSource source)
        {
            var tokens = source.Tokens;
            if (!IsValid(tokens))
            {
                return ParseExpressionResult.Invalid<VariableDeclarationExpression>("Invalid expression.");
            }

            var type = VariableType.Parse(tokens.TokenValue(0));
            var name = tokens.TokenValue(1);
            var assignment = tokens.Length > 3 ? ExpressionFactory.Parse(source.File, tokens.TokensFrom(3), source.Line) : null;
            var reference = source.CreateReference();

            var expression = new VariableDeclarationExpression(type, name, assignment, source, reference);

            return ParseExpressionResult.Success(expression);
        }

        public static bool IsValid(TokenList tokens)
        {
            return tokens.Length == 2
                   && tokens.IsTokenType<StringLiteralToken>(0)
                   && tokens.IsTokenType<StringLiteralToken>(1)
                   || tokens.Length >= 4
                   && tokens.IsTokenType<StringLiteralToken>(0)
                   && tokens.IsTokenType<StringLiteralToken>(1)
                   && tokens.OperatorToken(2, OperatorType.Assignment);
        }

        protected override IEnumerable<INode> GetChildren()
        {
            if (Assignment != null)
            {
                yield return Assignment;
            }
        }

        protected override void Validate(IValidationContext context)
        {
            context.FunctionCodeContext.EnsureVariableUnique(this, VariableName);
        }
    }
}