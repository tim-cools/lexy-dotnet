using System.Collections.Generic;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public class Comments : IComponent
    {
        public IList<string> Lines { get; } = new List<string>();

        public IComponent Parse(ParserContext parserContext)
        {
            Lines.Add(parserContext.CurrentLine.TrimmedContent.TrimStart(TokenNames.CommentChar));
            return this;
        }
    }
}