using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lexy.Poc.Core.Parser.Tokens
{
    public class DateTimeLiteral : ParsableToken, ILiteralToken
    {
        private int index = 0;

        //format d"0123/56/89 12:45:78"
        private static readonly int[] digitIndexes = { 0, 1, 2, 3, 5, 6, 8, 9, 11, 12, 14, 15, 17, 18 };
        private static readonly int[] slashIndexes = { 4, 7 };
        private static readonly int[] spaceIndexes = { 10 };
        private static readonly int[] colonIndexes = { 13, 16 };
        private static readonly int[] validLengths = { 19 };

        private const string DateTimeFormat = "yyyy/MM/dd HH:mm:ss";

        private readonly IList<Func<char, ParseTokenResult>> validators;

        public DateTime DateTimeValue { get; set; }

        public DateTimeLiteral()
        {
            validators = new List<Func<char, ParseTokenResult>>
            {
                value => Validate(value, char.IsDigit(value), digitIndexes),
                value => Validate(value, TokenValues.Slash, slashIndexes),
                value => Validate(value, TokenValues.Colon, colonIndexes),
                value => Validate(value, TokenValues.Space, spaceIndexes)
            };
        }

        public override ParseTokenResult Parse(char value, IParserContext context)
        {
            if (value == TokenValues.Quote)
            {
                if (!validLengths.Contains(Value.Length))
                {
                    var expectedLengths = string.Join(",", validLengths.Select(value => value.ToString()).ToArray());
                    return ParseTokenResult.Invalid(
                        $"Invalid date time format length '{Value.Length}'. Expected: '{expectedLengths}'");
                }

                ParseValue();
                return ParseTokenResult.Finished(true);
            }

            foreach (var validator in validators)
            {
                var result = validator(value);
                if (result != null)
                {
                    AppendValue(value);
                    index++;
                    return result;
                }
            }

            return ParseTokenResult.Invalid($@"Unexpected character: '{value}'. Format: d""2024/12/18 14:17:30""");
        }

        private void ParseValue()
        {
            DateTimeValue = DateTime.ParseExact(Value, DateTimeFormat, CultureInfo.InvariantCulture);
        }

        private ParseTokenResult Validate(char value, char match, int[] indexes) => Validate(value, value == match, indexes);

        private ParseTokenResult Validate(char value, bool match, int[] indexes)
        {
            if (!match) return null;

            if (!indexes.Contains(index))
                return ParseTokenResult.Invalid($@"Unexpected character: '{value}'. Format: d""2024/12/18 14:17:30""");

            return ParseTokenResult.InProgress();
        }

        public override ParseTokenResult Finalize(IParserContext parserContext)
        {
            return ParseTokenResult.Invalid(
                $@"Unexpected end of line. Closing quote expected. Format: d""2024/12/18 14:17:30""");
        }

        public override string ToString() => DateTimeValue.ToString(DateTimeFormat, CultureInfo.InvariantCulture);
    }
}