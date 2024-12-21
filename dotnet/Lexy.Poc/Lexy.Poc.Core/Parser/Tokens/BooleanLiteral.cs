using System;

namespace Lexy.Poc.Core.Parser.Tokens
{
    public class BooleanLiteral : Token, ILiteralToken
    {
        public bool BooleanValue { get; }

        public override string Value => BooleanValue ? TokenValues.BooleanTrue : TokenValues.BooleanFalse;

        public BooleanLiteral(bool value)
        {
            BooleanValue = value;
        }

        public static BooleanLiteral Parse(string value)
        {
            if (value == TokenValues.BooleanTrue)
            {
                return new BooleanLiteral(true);
            }

            if (value == TokenValues.BooleanFalse)
            {
                return new BooleanLiteral(false);

            }

            throw new InvalidOperationException("Couldn't parse boolean: " + value);
        }

        public static bool IsValid(string value)
        {
            return value == TokenValues.BooleanTrue || value == TokenValues.BooleanFalse;
        }
    }

}