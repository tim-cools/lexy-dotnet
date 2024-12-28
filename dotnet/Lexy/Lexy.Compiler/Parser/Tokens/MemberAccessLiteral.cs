using System;
using Lexy.Compiler.Language;
using Lexy.Compiler.Language.Scenarios;
using Lexy.Compiler.Language.Types;
using Lexy.RunTime;

namespace Lexy.Compiler.Parser.Tokens
{
    public class MemberAccessLiteral : Token, ILiteralToken
    {
        private readonly string[] parts;
        public override string Value { get; }

        public string Parent => parts.Length >= 1 ? parts[0] : null;
        public string Member => parts.Length >= 2 ? parts[1] : null;

        public string[] Parts => parts;

        public object TypedValue => parts;

        public MemberAccessLiteral(string value, TokenCharacter character) : base(character)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            parts = Value.Split(TokenValues.MemberAccess);
        }

        public override string ToString() => Value;

        public VariableType DeriveType(IValidationContext context)
        {
            var variableReference = new VariableReference(parts);
            var variableType = context.VariableContext.GetVariableType(variableReference, context);
            if (variableType != null) return variableType;

            if (parts.Length != 2) return null;

            var rootType = context.RootNodes.GetType(Parent);
            return rootType is not ITypeWithMembers typeWithMembers ? null : typeWithMembers.MemberType(Member, context);
        }
    }
}