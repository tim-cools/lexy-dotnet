using System;
using System.Collections.Generic;
using System.IO;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language.Expressions
{
    public abstract class Expression : Node
    {
        public ExpressionSource Source { get; }

        protected Expression(ExpressionSource source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public override string ToString()
        {
            var writer = new StringWriter();
            foreach (var token in Source.Tokens)
            {
                writer.Write(token.Value);
            }
            return writer.ToString();
        }
    }

    public class ExpressionSource
    {
        public Line Line { get; }
        public TokenList Tokens { get; }

        public ExpressionSource(Line line, TokenList tokens)
        {
            Line = line;
            Tokens = tokens;
        }
    }
}