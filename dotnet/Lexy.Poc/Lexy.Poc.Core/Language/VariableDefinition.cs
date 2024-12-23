using System;
using System.Collections.Generic;
using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;

namespace Lexy.Poc.Core.Language
{
    public class VariableDefinition : Node
    {
        public ILiteralToken Default { get; }
        public VariableType Type { get; }
        public string Name { get; }

        private VariableDefinition(string name, VariableType type,
            SourceReference reference, ILiteralToken @default = null) : base(reference)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Name = name ?? throw new ArgumentNullException(nameof(name));
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

            var variableType = VariableType.Parse(type);
            if (variableType == null) return null;

            if (tokens.Length == 2)
            {
                return new VariableDefinition(name, variableType, context.LineStartReference());
            }

            if (tokens.Token<OperatorToken>(2).Type != OperatorType.Assignment)
            {
                context.Logger.Fail(context.TokenReference(2), "Invalid variable declaration token. Expected '='.");
                return null;
            }
            if (tokens.Length != 4)
            {
                context.Logger.Fail(context.LineEndReference(), "Invalid variable declaration token. Expected default literal token.");
                return null;
            }

            var defaultValue = tokens.LiteralToken(3);
            return new VariableDefinition(name, variableType, context.LineStartReference(), defaultValue);
        }

        protected override IEnumerable<INode> GetChildren()
        {
            yield break;
        }

        protected override void Validate(IValidationContext context)
        {
            context.FunctionCodeContext.EnsureVariableUnique(this, Name);

            switch (Type)
            {
                case CustomVariableType customVariableType:
                    ValidateCustomVariableType(context, customVariableType);
                    break;

                case PrimitiveVariableType primitiveVariableType:
                    ValidatePrimitiveVariableType(context, primitiveVariableType);
                    break;

                default:
                    throw new InvalidOperationException("Invalid Type: " + Type.GetType());
            }
        }

        private void ValidateCustomVariableType(IValidationContext context, CustomVariableType customVariableType)
        {
            if (!context.Nodes.ContainsEnum(customVariableType.TypeName))
            {
                context.Logger.Fail(Reference, $"Unknown type: '{customVariableType.TypeName}'");
                return;
            }

            if (Default == null) return;

            if (!(Default is MemberAccessLiteral memberAccessLiteral))
            {
                context.Logger.Fail(Reference, $"Invalid default value '{Default}'. (type: '{customVariableType.TypeName}')");
            }
            else
            {
                var parts = memberAccessLiteral.GetParts();
                var enumDeclaration = context.Nodes.GetEnum(customVariableType.TypeName);
                if (parts.Length != 2)
                {
                    context.Logger.Fail(Reference, $"Invalid default value '{Default}'. (type: '{customVariableType.TypeName}')");
                }
                if (parts[0] != customVariableType.TypeName)
                {
                    context.Logger.Fail(Reference, $"Invalid default value '{Default}'. Invalid enum type. (type: '{customVariableType.TypeName}')");
                }
                if (!enumDeclaration.ContainsMember(parts[1]))
                {
                    context.Logger.Fail(Reference, $"Invalid default value '{Default}'. Invalid member. (type: '{customVariableType.TypeName}')");
                }
            }
        }

        private void ValidatePrimitiveVariableType(IValidationContext context, PrimitiveVariableType primitiveVariableType)
        {
            if (Default == null) return;

            switch (primitiveVariableType.Type)
            {
                case TypeNames.Number:
                    ValidateDefaultLiteral<NumberLiteralToken>(context, primitiveVariableType);
                    break;

                case TypeNames.String:
                    ValidateDefaultLiteral<QuotedLiteralToken>(context, primitiveVariableType);
                    break;

                case TypeNames.Boolean:
                    ValidateDefaultLiteral<BooleanLiteral>(context, primitiveVariableType);
                    break;

                case TypeNames.DateTime:
                    ValidateDefaultLiteral<DateTimeLiteral>(context, primitiveVariableType);
                    break;

                default:
                    throw new InvalidOperationException($"Unexpected type: {primitiveVariableType.Type}");
            }
        }

        private void ValidateDefaultLiteral<T>(IValidationContext context, PrimitiveVariableType primitiveVariableType)
            where T : ILiteralToken
        {
            if (!(Default is T))
            {
                context.Logger.Fail(Reference, $"Invalid default value '{Default}'. (type: '{primitiveVariableType.Type}')");
            }
        }
    }
}