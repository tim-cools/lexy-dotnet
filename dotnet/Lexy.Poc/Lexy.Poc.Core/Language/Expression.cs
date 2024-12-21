using System.IO;
using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;

namespace Lexy.Poc.Core.Language
{
    public class Expression
    {
        public Line SourceLine { get; }
        public Token[] Tokens { get; }

        private Expression(Line sourceLine, Token[] tokens)
        {
            SourceLine = sourceLine;
            Tokens = tokens;
        }

        public static Expression Parse(Line sourceLine, Token[] tokens)
        {
            var expression = new Expression(sourceLine, tokens);
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