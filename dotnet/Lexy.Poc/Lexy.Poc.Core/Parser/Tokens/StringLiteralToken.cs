
namespace Lexy.Poc.Core.Parser.Tokens
{
    public class StringLiteralToken : Token, ILiteralToken
    {
        public override string Value { get; }

        public StringLiteralToken(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;
    }
}