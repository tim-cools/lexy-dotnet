
namespace Lexy.Poc.Core.Parser
{
    public class StringLiteralToken : Token, ILiteralToken
    {
        public override string Value { get; }

        public StringLiteralToken(string value)
        {
            Value = value;
        }
    }
}