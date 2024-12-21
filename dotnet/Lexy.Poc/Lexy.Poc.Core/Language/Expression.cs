using System.IO;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public class Expression
    {
        public Token[] Tokens { get; }

        private Expression(Token[] tokens)
        {
            Tokens = tokens;
        }

        public static Expression Parse(Token[] tokens)
        {
            var expression = new Expression(tokens);
            return expression;
        }

        public override string ToString()
        {
            var writer = new StringWriter();
            foreach (var token in Tokens)
            {
                writer.Write(token.Value);
            }
            return writer.ToString();
        }
    }
}