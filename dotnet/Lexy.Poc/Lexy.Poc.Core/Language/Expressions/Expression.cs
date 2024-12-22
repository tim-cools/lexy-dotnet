using System.IO;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language.Expressions
{
    public class Expression
    {
        public Line SourceLine { get; }
        public TokenList Tokens { get; }

        protected Expression(Line sourceLine, TokenList tokens)
        {
            SourceLine = sourceLine;
            Tokens = tokens;
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