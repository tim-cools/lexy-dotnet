using Lexy.Compiler.Parser;

namespace Lexy.Compiler.Language
{
    public abstract class ParsableNode : Node, IParsableNode
    {
        public abstract IParsableNode Parse(IParserContext context);

        protected ParsableNode(SourceReference reference) : base(reference)
        {
        }
    }
}