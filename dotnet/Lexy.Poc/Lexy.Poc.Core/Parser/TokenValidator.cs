using System;
using Lexy.Poc.Core.Parser.Tokens;

namespace Lexy.Poc.Core.Parser
{
    public class TokenValidator
    {
        private readonly ParserContext parserContext;
        private readonly Token[] tokens;
        private bool errorsExpected;

        public bool IsValid { get; private set; }

        public TokenValidator(ParserContext parserContext)
        {
            this.parserContext = parserContext;
            tokens = parserContext.CurrentLine.Tokens;

            IsValid = true;
        }

        public TokenValidator Count(int count)
        {
            if (tokens.Length != count)
            {
                parserContext.Fail($"Invalid number of tokens '{tokens.Length}', should be '{count}'.");
                IsValid = false;
            }

            return this;
        }

        public TokenValidator CountMinimum(int count)
        {
            if (tokens.Length < count)
            {
                parserContext.Fail($"Invalid number of tokens '{tokens.Length}', should be at least '{count}'.");
                IsValid = false;
            }

            return this;
        }

        public TokenValidator Keyword(int index, string keyword = null)
        {
            Type<KeywordToken>(index);
            if (keyword != null)
            {
                Value(index, keyword);
            }
            return this;
        }

        /* public TokenValidator Literal(int index, string literal = null)
        {
            Type<LiteralToken>(index);
            if (literal != null)
            {
                Value(index, literal);
            }
            return this;
        } */

        /*
        public TokenValidator TypeLiteral(int index, Types? type = null)
        {
            var token = ValidateType<TypeLiteralToken>(index);
            if (token != null && type.HasValue && token.TypeValue != type.Value)
            {
                parserContext.Fail($"Invalid token value. Expected: '{type}' Actual: '{token.Value}'");
                IsValid = false;
            }
            return this;
        } */

        public TokenValidator StringLiteral(int index, string value = null)
        {
            Type<StringLiteralToken>(index);
            if (value != null)
            {
                Value(index, value);
            }
            return this;
        }

        public TokenValidator Operator(int index, OperatorType operatorType)
        {
            if (index >= tokens.Length)
            {
                parserContext.Fail($"Invalid token {index} value. Only {tokens.Length} tokens.");
                IsValid = false;
                return this;
            }

            Type<OperatorToken>(index);
            var token = tokens[index] as OperatorToken;
            if (token.Type != operatorType)
            {
                parserContext.Fail($"Invalid operator token {index} value. Expected: '{operatorType}' Actual: '{token.Type}'");
                IsValid = false;
            }
            return this;
        }

        public TokenValidator MemberAccess(int index, string value = null)
        {
            Type<MemberAccessLiteral>(index);
            if (value != null)
            {
                Value(index, value);
            }
            return this;
        }

        public TokenValidator Comment(int index)
        {
            Type<CommentToken>(index);
            return this;
        }

        public TokenValidator QuotedString(int index, string literal = null)
        {
            Type<QuotedLiteralToken>(index);
            return this;
        }

        public TokenValidator IntLiteral(int index, int value)
        {
            var token = ValidateType<IntLiteralToken>(index);
            if (token != null && token.NumberValue != value)
            {
                parserContext.Fail($"Invalid token {index} value. Expected: '{value}' Actual: '{token.Value}'");
                IsValid = false;
            }
            return this;
        }

        public TokenValidator NumberLiteral(int index, decimal value)
        {
            var token = ValidateType<NumberLiteralToken>(index);
            if (token != null && token.NumberValue != value)
            {
                parserContext.Fail($"Invalid token {index} value. Expected: '{value}' Actual: '{token.Value}'");
                IsValid = false;
            }
            return this;
        }

        public TokenValidator Boolean(int index, bool value)
        {
            var token = ValidateType<BooleanLiteral>(index);
            if (token != null && token.BooleanValue != value)
            {
                parserContext.Fail($"Invalid token {index} value. Expected: '{value}' Actual: '{token.Value}'");
                IsValid = false;
            }
            return this;
        }

        public TokenValidator DateTime(int index, int year, int month, int day, int hours, int minutes, int seconds)
        {
            var token = ValidateType<DateTimeLiteral>(index);
            var value = new System.DateTime(year, month, day, hours, minutes, seconds);
            if (token != null && token.DateTimeValue != value)
            {
                parserContext.Fail($"Invalid token {index} value. Expected: '{value}' Actual: '{token.Value}'");
                IsValid = false;
            }
            return this;
        }

        public TokenValidator Type<T>(int index) where T: Token
        {
            ValidateType<T>(index);
            return this;
        }

        public TokenValidator IsLiteralToken(int index)
        {
            if (index >= tokens.Length)
            {
                parserContext.Fail($"Invalid token '{index}' Length: '{tokens.Length}'");
                IsValid = false;

                return this;
            }

            var token = tokens[index] as ILiteralToken;
            if (token == null)
            {
                parserContext.Fail($"Invalid token {index}  type. Expected: 'ILiteralToken' Actual: '{tokens[index].GetType().Name}'");
                IsValid = false;

                return this;
            }
            return this;
        }

        private T ValidateType<T>(int index) where T: Token
        {
            if (index >= tokens.Length)
            {
                parserContext.Fail($"Invalid token '{index}' Length: '{tokens.Length}'");
                IsValid = false;

                return null;
            }

            var token = tokens[index];
            var type = token.GetType();
            if (type != typeof(T))
            {
                parserContext.Fail($"Invalid token {index}  type. Expected: '{typeof(T).Name}' Actual: '{type.Name}'");
                IsValid = false;

                return null;
            }
            return (T) token;
        }

        public TokenValidator Value(int index, string expectedValue)
        {
            if (index >= tokens.Length)
            {
                parserContext.Fail($"Invalid token {index} value. Only {tokens.Length} tokens.");
                IsValid = false;
                return this;
            }
            var token = tokens[index];
            if (token.Value != expectedValue)
            {
                parserContext.Fail($"Invalid token  {index} value. Expected: '{expectedValue}' Actual: '{token.Value}'");
                IsValid = false;
            }
            return this;
        }


        public TokenValidator ExpectError(string expectedError)
        {
            errorsExpected = true;
            if (!parserContext.HasErrorMessage(expectedError))
            {
                parserContext.Fail($"Error expected but not found: " + expectedError + " - Errors: " +
                                   parserContext.FormatMessages());
                IsValid = false;
            }

            return this;
        }

        public void Assert()
        {
            if (!errorsExpected && parserContext.HasErrors())
            {
                throw new InvalidOperationException(parserContext.FormatMessages());
            }

            if (!IsValid)
            {
                throw new InvalidOperationException(parserContext.FormatMessages());
            }
        }
    }
}