using Lexy.Poc.Core.Language.Expressions;
using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;

namespace Lexy.Poc.Core.Language
{
    public class AssignmentDefinition
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
            var value = ExpressionFactory.Parse(context, line.Tokens.TokensFrom(2));
            return value != null ? new AssignmentDefinition(name, value) : null;
        }
    }

    public class EnumMember
    {
        public NumberLiteralToken Value { get; }
        public string Name { get; }

        public EnumMember(string name, NumberLiteralToken value = null)
        {
            Value = value;
            Name = name;
        }

        public static EnumMember Parse(IParserContext context)
        {
            var valid = context.ValidateTokens<AssignmentDefinition>()
                .CountMinimum(1)
                .StringLiteral(0)
                .IsValid;

            if (!valid) return null;

            var line = context.CurrentLine;
            var name = line.Tokens.TokenValue(0);
            if (context.CurrentLine.Tokens.Length == 1)
            {
                return new EnumMember(name);
            }
            if (context.CurrentLine.Tokens.Length == 3)
            {
                valid = context.ValidateTokens<AssignmentDefinition>()
                    .Operator(1, OperatorType.Assignment)
                    .IsValid;
                if (!valid) return null;

                var value = line.Tokens.Token<NumberLiteralToken>(2);
                return new EnumMember(name, value);
            }

            context.Logger.Fail("Invalid number of tokens: " + context.CurrentLine.Tokens.Length  + ". Should be 1 or 3.");

            return null;
        }
    }
}