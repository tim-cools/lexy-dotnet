using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexy.Poc.Core.Parser
{
    public class ParserContext
    {
        private IList<string> failedMessages = new List<string>();
        private readonly ITokenizer tokenizer = new Tokenizer();


        public void Fail(string message)
        {
            failedMessages.Add($"{CurrentLine?.Index}: {message}");
        }

        internal Line CurrentLine { get; private set; }

        public string LastError()
        {
            return failedMessages.LastOrDefault();
        }

        public void ProcessLine(Line line)
        {
            CurrentLine = line ?? throw new ArgumentNullException(nameof(line));
            CurrentLine.Tokenize(tokenizer, this);
        }

        public bool ValidateTokens(Action<TokenValidator> tokenValidator)
        {
            var validator = new TokenValidator(this);
            tokenValidator(validator);
            return validator.Success;
        }

        public string ErrorMessages()
        {
            return string.Join(Environment.NewLine, failedMessages);
        }
    }

    public class TokenValidator
    {
        private readonly ParserContext parserContext;
        private readonly Token[] tokens;

        public bool Success { get; private set; }

        public TokenValidator(ParserContext parserContext)
        {
            this.parserContext = parserContext;
            tokens = parserContext.CurrentLine.Tokens;
            Success = true;
        }

        public TokenValidator Count(int count)
        {
            if (tokens.Length != count)
            {
                parserContext.Fail($"Invalid number of tokens '{tokens.Length}', should be '{count}'.");
                Success = false;
            }

            return this;
        }

        public TokenValidator Type<T>(int index)
        {
            var token = tokens[index];
            var type = token.GetType();
            if (type != typeof(T))
            {
                parserContext.Fail($"Invalid token type. Expected: '{typeof(T).Name}' Actual: '{type.Name}'");
                Success = false;
            }
            return this;
        }

        public TokenValidator Value(int index, string expectedValue)
        {
            var token = tokens[index];
            if (token.Value != expectedValue)
            {
                parserContext.Fail($"Invalid token value. Expected: '{expectedValue}' Actual: '{token.Value}'");
                Success = false;
            }
            return this;
        }
    }
}