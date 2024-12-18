using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexy.Poc.Core.Parser
{
    public class Tokenizer : ITokenizer
    {
        private readonly IDictionary<char, Func<char, ParsableToken>> knownTokens =
            new Dictionary<char, Func<char, ParsableToken>>
            {
                { TokenValues.CommentChar, value => new CommentToken(value) },
                { TokenValues.Quote, value => new QuotedLiteralToken(value) },
                { TokenValues.Assignment, value => new AssignmentOperatorToken() },
                { TokenValues.TableSeparator, value => new TableSeparatorToken() },
            };

        private readonly IDictionary<Func<char, bool>, Func<char, ParsableToken>> tokensValidators =
            new Dictionary<Func<char, bool>, Func<char, ParsableToken>>
            {
                { char.IsDigit, value => new NumericLiteralToken(value)},
                { char.IsLetter, value => new BuildLiteralToken(value)},
                { char.IsWhiteSpace, value => new WhitespaceToken(value)}
            };

        public Token[] Tokenize(Line line, ParserContext parserContext, out bool errors)
        {
            if (line == null) throw new ArgumentNullException(nameof(line));

            errors = false;
            var tokens = new List<Token>();
            ParsableToken current = null;

            for (var index = 0; index < line.Content.Length; index++)
            {
                var value = line.Content[index];
                var valueProcessed = false;
                if (current != null)
                {
                    var result = current.Parse(value, parserContext);
                    if (result.Status == TokenStatus.InvalidToken)
                    {
                        parserContext.Fail($"Invalid token {index}: {result.ValidationError}");
                        errors = true;
                        break;
                    }

                    if (result.Status == TokenStatus.Finished)
                    {
                        tokens.Add(result.NewToken ?? current);
                        current = null;
                        valueProcessed = result.CharProcessed;
                    }
                    else if (result.Status == TokenStatus.InProgress && result.NewToken != null)
                    {
                        var parsableToken = result.NewToken as ParsableToken;
                        if (parsableToken == null)
                        {
                            throw new InvalidOperationException(
                                "New token can only be a parsable token when in progress");
                        }
                        current = parsableToken;
                    }
                }

                if (current == null && !valueProcessed)
                {
                    current = StartToken(value, index, parserContext);
                }
            }

            if (!errors && current != null)
            {
                var result = current.Finalize(parserContext);
                if (result.Status != TokenStatus.Finished)
                {
                    parserContext.Fail($"Invalid token at end of line: {result.Status} ({result.ValidationError})");
                    errors = true;
                }
                else
                {
                    tokens.Add(result.NewToken ?? current);
                }
            }

            return DiscardWhitespace(tokens);
        }

        private static Token[] DiscardWhitespace(List<Token> tokens)
        {
            return tokens.Where(token => !(token is WhitespaceToken)).ToArray();
        }

        private ParsableToken StartToken(char value, int index, ParserContext parserContext)
        {
            if (knownTokens.ContainsKey(value))
            {
                return knownTokens[value](value);
            }

            foreach (var validator in tokensValidators)
            {
                if (validator.Key(value))
                {
                    return validator.Value(value);
                }
            }

            parserContext.Fail($"Invalid character at {index} '{value}'");
            return null;
        }
    }
}