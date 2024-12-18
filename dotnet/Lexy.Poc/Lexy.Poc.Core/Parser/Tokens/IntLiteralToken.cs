using System.Globalization;

namespace Lexy.Poc.Core.Parser
{
    public class IntLiteralToken : Token, ILiteralToken
    {
        public int NumberValue { get; }

        public override string Value => NumberValue.ToString(CultureInfo.InvariantCulture);

        public IntLiteralToken(int value)
        {
            NumberValue = value;
        }

        public static IntLiteralToken Parse(string value)
        {
            var number = int.Parse(value);
            return new IntLiteralToken(number);
        }
    }
}