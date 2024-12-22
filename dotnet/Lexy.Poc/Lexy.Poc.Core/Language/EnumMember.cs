using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;

namespace Lexy.Poc.Core.Language
{
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