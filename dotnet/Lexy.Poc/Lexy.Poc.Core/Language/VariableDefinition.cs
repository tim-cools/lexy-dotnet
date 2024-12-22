using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;

namespace Lexy.Poc.Core.Language
{
    public class VariableDefinition
    {
        public ILiteralToken Default { get; }
        public VariableType Type { get; }
        public string Name { get; }

        private VariableDefinition(string name, VariableType type, ILiteralToken @default = null)
        {
            Type = type;
            Name = name;
            Default = @default;
        }

        public static VariableDefinition Parse(IParserContext context)
        {
            var line = context.CurrentLine;
            if (line.IsEmpty()) return null;

            var result = context.ValidateTokens<VariableDefinition>()
                .CountMinimum(2)
                .StringLiteral(0)
                .StringLiteral(1)
                .IsValid;

            if (!result) return null;

            var tokens = line.Tokens;
            var name = tokens.TokenValue(1);
            var type = tokens.TokenValue(0);

            var variableType = VariableType.Parse(type, context);
            if (variableType == null) return null;

            if (tokens.Length == 2)
            {
                return new VariableDefinition(name, variableType);
            }

            if (tokens.Token<OperatorToken>(2).Type != OperatorType.Assignment)
            {
                context.Logger.Fail("Invalid variable declaration token. Expected '='.", context.CurrentComponent);
                return null;
            }
            if (tokens.Length != 4)
            {
                context.Logger.Fail("Invalid variable declaration token. Expected default literal token.", context.CurrentComponent);
                return null;
            }

            var @default = tokens.LiteralToken(3);
            return new VariableDefinition(name, variableType, @default);
        }
    }
}