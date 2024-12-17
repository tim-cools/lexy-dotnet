using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexy.Poc.Core.Parser
{
    public class Tokenizer : ITokenizer
    {
        private IDictionary<char, Func<char, Token>> knownTokens = new Dictionary<char, Func<char, Token>>();

        public Token[] Tokenize(Line line, ParserContext parserContext)
        {
            if (line == null) throw new ArgumentNullException(nameof(line));

            var tokens = new List<Token>();
            Token current = null;

            for (var index = 0; index < line.Content.Length; index++)
            {
                var value = line.Content[index];
                if (current != null)
                {
                    var result = current.Parse(value, parserContext);
                    if (result.Status == TokenStatus.InvalidToken)
                    {
                        break;
                    }
                    if (result.Status == TokenStatus.Finished)
                    {
                        tokens.Add(result.NewToken ?? current);
                        current = null;
                    }
                }

                current ??= StartToken(value, index, parserContext);
            }

            if (current != null)
            {
                var result = current.Finalize(parserContext);
                if (result.Status == TokenStatus.Finished)
                {
                    tokens.Add(result.NewToken ?? current);
                }
            }

            return TrimWhitespace(tokens).ToArray();
        }

        private static IEnumerable<Token> TrimWhitespace(List<Token> tokens)
        {
            return tokens.Where(token => !(token is WhitespaceToken));
        }

        private Token StartToken(char value, int index, ParserContext parserContext)
        {
            if (value == TokenNames.CommentChar)
            {
                return new CommentToken(value);
            }
            if (char.IsLetter(value))
            {
                return new LiteralToken(value);
            }
            if (char.IsDigit(value))
            {
                return new NumericToken(value);
            }
            if (char.IsWhiteSpace(value))
            {
                return new WhitespaceToken(value);
            }

            if (knownTokens.ContainsKey(value))
            {
                return knownTokens[value](value);
            }

            parserContext.Fail($"Invalid character at {index} '{value}'");
            return null;
        }
    }
}