using System.Linq;

namespace Lexy.Poc.Core.Parser.Tokens
{
    public class BuildLiteralToken : ParsableToken
    {
        private static char[] terminatorValues = new[]
        {
            TokenValues.Space,
            TokenValues.OpenParentheses,
            TokenValues.OpenBrackets,
            TokenValues.CloseParentheses,
            TokenValues.CloseBrackets,
        };

        private bool hasMemberAccessor;
        private bool lastMemberAccessor;

        public BuildLiteralToken(char value) : base(value)
        {
        }

        public override ParseTokenResult Parse(char value, IParserContext parserContext)
        {
            if (terminatorValues.Contains(value))
            {
                return ParseTokenResult.Finished(false, SealLiteral());
            }

            if (value == '.')
            {
                if (lastMemberAccessor)
                {
                    return ParseTokenResult.Invalid($"Unexpected character: '{value}'. Member accessor should be followed by member name.");
                }

                hasMemberAccessor = true;
                lastMemberAccessor = true;
                AppendValue(value);
                return ParseTokenResult.InProgress();
            }

            if (char.IsLetterOrDigit(value) || value == ':')
            {
                lastMemberAccessor = false;

                AppendValue(value);
                return ParseTokenResult.InProgress();
            }

            if (value == TokenValues.Quote && Value == TokenValues.DateTimeStarter)
            {
                return ParseTokenResult.InProgress(new DateTimeLiteral());
            }

            return ParseTokenResult.Invalid($"Unexpected character: '{value}'");
        }

        public override ParseTokenResult Finalize(IParserContext parserContext)
        {
            if (lastMemberAccessor)
            {
                return ParseTokenResult.Invalid("Unexpected end of line. Member accessor should be followed by member name.");
            }

            return ParseTokenResult.Finished(true, SealLiteral());
        }

        private Token SealLiteral()
        {
            var value = Value;
            if (Keywords.Contains(value))
            {
                return new KeywordToken(value);
            }
            /* if (TypeNames.Contains(value))
            {
                return TypeLiteralToken.Parse(value);
            } */
            if (BooleanLiteral.IsValid(value))
            {
                return BooleanLiteral.Parse(value);
            }

            if (hasMemberAccessor)
            {
                return new MemberAccessLiteral(value);
            }

            return new StringLiteralToken(value);
        }
    }
}