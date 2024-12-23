using System;

namespace Lexy.Poc.Core.Parser.Tokens
{
    public class MemberAccessLiteral : Token, ILiteralToken
    {
        public override string Value { get; }

        public MemberAccessLiteral(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string[] GetParts() => Value.Split(TokenValues.MemberAccess);

        public override string ToString() => Value;
    }
}