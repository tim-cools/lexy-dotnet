using System;

namespace Lexy.Poc.Core.Parser.Tokens
{
    public abstract class Token : IToken
    {
        public TokenCharacter FirstCharacter { get; }

        public abstract string Value { get; }

        protected Token(TokenCharacter firstCharacter)
        {
            FirstCharacter = firstCharacter ?? throw new ArgumentNullException(nameof(firstCharacter));
        }
    }

    public interface IToken
    {
        TokenCharacter FirstCharacter { get; }
    }

    public class TokenCharacter
    {
        public int Position { get; }
        public char Value { get; }

        public TokenCharacter(char value, int position)
        {
            Value = value;
            Position = position;
        }
    }
}