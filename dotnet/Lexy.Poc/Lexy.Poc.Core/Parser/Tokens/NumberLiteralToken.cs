using System.Globalization;

namespace Lexy.Poc.Core.Parser
{
    internal class NumberLiteralToken : Token, ILiteralToken
    {
        public decimal NumberValue { get; }

        public override string Value => NumberValue.ToString(CultureInfo.InvariantCulture);

        public NumberLiteralToken(decimal value)
        {
            NumberValue = value;
        }

        public static NumberLiteralToken Parse(string value)
        {
            var number = decimal.Parse(value);
            return new NumberLiteralToken(number);
        }
    }
}