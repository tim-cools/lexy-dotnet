
namespace Lexy.Poc.Core.Parser.Tokens
{
    public class StringLiteralToken : Token, ILiteralToken
    {
        public override string Value { get; }

        public StringLiteralToken(string value, TokenCharacter character) : base(character)
        {
            Value = value;
        }

        public override string ToString() => Value;
    }
}