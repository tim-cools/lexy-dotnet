using System.Collections.Generic;
using Lexy.Poc.Core.Language.Expressions;
using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;

namespace Lexy.Poc.Core.Language
{
    public class AssignmentDefinition : Node
    {
        public Expression Expression { get; }
        public string Name { get; }

        public AssignmentDefinition(string name, Expression expression)
        {
            Expression = expression;
            Name = name;
        }

        public static AssignmentDefinition Parse(IParserContext context)
        {
            var valid = context.ValidateTokens<AssignmentDefinition>()
                .CountMinimum(3)
                .StringLiteral(0)
                .Operator(1, OperatorType.Assignment)
                .IsValid;

            if (!valid) return null;

            var line = context.CurrentLine;
            var name = line.Tokens.TokenValue(0);
            var value = ExpressionFactory.Parse(line.Tokens.TokensFrom(2), line);
            return value != null ? new AssignmentDefinition(name, value) : null;
        }

        protected override IEnumerable<INode> GetChildren()
        {
            yield return Expression;
        }

        protected override void Validate(IValidationContext context)
        {
        }
    }
}