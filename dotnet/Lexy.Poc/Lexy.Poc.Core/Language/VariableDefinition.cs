using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;

namespace Lexy.Poc.Core.Language
{
    public class VariableDefinition
    {
        public Token Default { get; }
        public string Type { get; }
        public string Name { get; }

        public VariableDefinition(string name, string type, Token @default = null)
        {
            Type = type;
            Name = name;
            Default = @default;
        }

        public static VariableDefinition Parse(ParserContext context)
        {
            var result = context.ValidateTokens<VariableDefinition>()
                .CountMinimum(2)
                .StringLiteral(0)
                .StringLiteral(1)
                .IsValid;

            if (!result) return null;

            var line = context.CurrentLine;
            var tokens = line.Tokens;
            var name = line.TokenValue(1);
            var type = line.TokenValue(0);

            if (tokens.Length == 2)
            {
                return new VariableDefinition(name, type);
            }

            if (line.Token<OperatorToken>(2).Type != OperatorType.Assignment)
            {
                context.Fail("Invalid variable declaration token. Expected '='.");
                return null;
            }
            if (tokens.Length != 4)
            {
                context.Fail("Invalid variable declaration token. Expected default literal token.");
                return null;
            }

            var @default = line.Token<Token>(3);
            return new VariableDefinition(name, type, @default);
        }
    }
}