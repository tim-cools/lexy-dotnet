namespace Lexy.Poc.Core.Parser.Tokens
{
    public class MemberAccessLiteral : Token
    {
        public override string Value { get; }

        public MemberAccessLiteral(string value)
        {
            Value = value;
        }
    }
}