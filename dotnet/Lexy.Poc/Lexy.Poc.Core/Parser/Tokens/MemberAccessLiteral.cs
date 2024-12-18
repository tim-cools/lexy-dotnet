namespace Lexy.Poc.Core.Parser
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