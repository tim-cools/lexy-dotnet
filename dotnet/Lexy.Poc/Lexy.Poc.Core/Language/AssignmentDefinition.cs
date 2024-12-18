using System;
using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;

namespace Lexy.Poc.Core.Language
{
    public class AssignmentDefinition
    {
        public string Value { get; }
        public string Name { get; }

        public AssignmentDefinition(string name, Expression value)
        {
            Value = value.Value;
            Name = name;
        }

        public static AssignmentDefinition Parse(ParserContext context)
        {
            var valid = context.ValidateTokens<AssignmentDefinition>()
                .CountMinimum(3)
                .StringLiteral(0)
                .Operator(1, OperatorType.Assignment)
                .IsValid;

            if (!valid) return null;

            var line = context.CurrentLine;
            var name = line.TokenValue(0);
            var value = Expression.Parse(line.TokensFrom(2));
            return value != null ? new AssignmentDefinition(name, value) : null;
        }
    }

    public class Expression
    {
        public string Value { get; private set; }

        public static Expression Parse(Token[] tokens)
        {
            var expression = new Expression();
            foreach (var token in tokens)
            {
                expression.Value += token.Value;
            }

            return expression;
        }
    }
}